using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Codeer.Friendly.DotNetExecutor;
using System.Threading.Tasks;

namespace Friendly.Core
{
    interface IAsyncInvoke
    {
        void Execute(Action method);
    }

    class SyncExecute : IAsyncInvoke
    {
        Action<Action> _invoke;
        public SyncExecute(Action<Action> invoke)
        {
            _invoke = invoke;
        }

        public void Execute(Action method)
        {
            _invoke(method);
        }
    }

    class AsyncExecute : IAsyncInvoke
    {
        Action<Action> _invoke;
        public AsyncExecute(Action<Action> invoke)
        {
            _invoke = invoke;
        }
        public void Execute(Action method)
        {
            Task.Factory.StartNew(()=> _invoke(method));
        }
    }
    
    static class FriendlyInvoker
    {
        internal static ReturnInfo Execute(Action<Action> invokeCore, VarPool varManager, TypeFinder typeFinder, ProtocolInfo info)
        {
            IAsyncInvoke invoke = new AsyncExecute(invokeCore);
            if (info.ProtocolType == ProtocolType.Operation)
            {
                invoke = new SyncExecute(invokeCore);
                var executeCheck = varManager.Add(false);
                var args = new List<object>(info.Arguments);
                args.Insert(0, executeCheck);
                info = new ProtocolInfo(info.ProtocolType, info.OperationTypeInfo, info.VarAddress,
                    info.TypeFullName, info.Operation, args.ToArray());
                try
                {
                    return Execute(invoke, varManager, typeFinder, info);
                }
                finally
                {
                    varManager.Remove(executeCheck);
                }
            }
            else
            {
                return Execute(invoke, varManager, typeFinder, info);
            }
        }
        
        internal static ReturnInfo Execute(IAsyncInvoke async, VarPool varManager, TypeFinder typeFinder, ProtocolInfo info)
        {
            switch (info.ProtocolType)
            {
                case ProtocolType.AsyncResultVarInitialize:
                case ProtocolType.VarInitialize:
                    return VarInitialize(varManager, info);
                case ProtocolType.VarNew:
                    return VarNew(async, varManager, typeFinder, info);
                case ProtocolType.BinOff:
                    return BinOff(varManager, info);
                case ProtocolType.GetValue:
                    return GetValue(varManager, info);
                case ProtocolType.SetValue:
                    return SetValue(varManager, info);
                case ProtocolType.GetElements:
                    return GetElements(varManager, info);
                case ProtocolType.Operation:
                case ProtocolType.AsyncOperation:
                    return AsyncOperation(async, varManager, typeFinder, info);
                case ProtocolType.IsEmptyVar:
                    return IsEmptyVar(varManager, info);
                default:
                    throw new InternalException();
            }
        }

        static ReturnInfo IsEmptyVar(VarPool varManager, ProtocolInfo info)
        {
            return new ReturnInfo(varManager.IsEmptyVar((VarAddress)info.Arguments[0]));
        }
        
        static ReturnInfo VarInitialize(VarPool varManager, ProtocolInfo info)
        {
            //arguments count is 1.
            if (info.Arguments.Length != 1)
            {
                throw new InternalException();
            }
            
            object[] args;
            ResolveArgs(varManager, info.Arguments, out args);

            //entry variable.
            return new ReturnInfo(varManager.Add(args[0]));
        }

        /// <summary>
        /// create object.
        /// </summary>
        static ReturnInfo VarNew(IAsyncInvoke async, VarPool varManager, TypeFinder typeFinder, ProtocolInfo info)
        {
            Type type = typeFinder.GetType(info.TypeFullName);
            if (type == null)
            {
                throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.UnknownTypeInfoFormat, info.TypeFullName));
            }
            
            object[] args;
            Type[] argTypesOri;
            ResolveArgs(varManager, info.Arguments, out args, out argTypesOri);
            Type[] argTypes = GetArgTypes(typeFinder, info.OperationTypeInfo, argTypesOri);

            //value type.
            if (argTypes.Length == 0 && type.IsValueType())
            {
                return new ReturnInfo(varManager.Add(Activator.CreateInstance(type)));
            }

            //resolve overload.
            ConstructorInfo[] constructorInfos = type.GetConstructors();
            List<ConstructorInfo> constructorList = new List<ConstructorInfo>();
            bool isObjectArrayArg = false;
            for (int i = 0; i < constructorInfos.Length; i++)
            {
                ParameterInfo[] paramInfos = constructorInfos[i].GetParameters();
                bool isPerfect;
                bool isObjectArrayArgTmp;
                if (IsMatchParameter(info.OperationTypeInfo != null, argTypes, paramInfos, out isPerfect, out isObjectArrayArgTmp))
                {
                    if (isPerfect)
                    {
                        constructorList.Clear();
                        constructorList.Add(constructorInfos[i]);
                        break;
                    }
                    constructorList.Add(constructorInfos[i]);
                }
                if (isObjectArrayArgTmp)
                {
                    isObjectArrayArg = true;
                }
            }

            //not found.
            if (constructorList.Count == 0)
            {
                if (isObjectArrayArg)
                {
                    throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorNotFoundConstractorFormatForObjectArray,
                        type.Name, MakeErrorInvokeArgInfo(argTypes)));
                }
                else
                {
                    throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorNotFoundConstractorFormat,
                        type.Name, MakeErrorInvokeArgInfo(argTypes)));
                }
            }
            if (constructorList.Count != 1)
            {
                //can't resolve overload.
                throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorManyFoundConstractorFormat,
                        type.Name, MakeErrorInvokeArgInfo(argTypes)));
            }

            //create.
            bool isCreated = false;//TODO
            object instance = null;
            async.Execute(() =>
            {
                instance = constructorList[0].Invoke(args);
                isCreated = true;
            });
            while (!isCreated) ;

            //resolve ref, out.
            ReflectArgsAfterInvoke(varManager, info.Arguments, args);

            //entry object.
            return new ReturnInfo(varManager.Add(instance));
        }
        
        static ReturnInfo BinOff(VarPool varManager, ProtocolInfo info)
        {
            varManager.Remove(info.VarAddress);
            return new ReturnInfo();
        }
        
        static ReturnInfo GetValue(VarPool varManager, ProtocolInfo info)
        {
            if (info.Arguments.Length != 0)
            {
                throw new InternalException();
            }
            return new ReturnInfo(varManager.GetVarAndType(info.VarAddress).Core);
        }
        
        static ReturnInfo SetValue(VarPool varManager, ProtocolInfo info)
        {
            if (info.Arguments.Length != 1)
            {
                throw new InternalException();
            }
            
            object[] args;
            ResolveArgs(varManager, info.Arguments, out args);
            
            varManager.SetObject(info.VarAddress, args[0]);
            return new ReturnInfo();
        }
        
        static ReturnInfo GetElements(VarPool varManager, ProtocolInfo info)
        {
            object obj = varManager.GetVarAndType(info.VarAddress).Core;

            IEnumerable enumerable = obj as IEnumerable;
            if (enumerable == null)
            {
                throw new InformationException(ResourcesLocal.Instance.HasNotEnumerable);
            }
            
            List<VarAddress> list = new List<VarAddress>();
            foreach (object element in enumerable)
            {
                list.Add(varManager.Add(element));
            }
            return new ReturnInfo(list.ToArray());
        }
        
        static ReturnInfo AsyncOperation(IAsyncInvoke async, VarPool varManager, TypeFinder typeFinder, ProtocolInfo info)
        {
            Type type;
            object obj;
            object[] args;
            Type[] argTypesOri;
            BindingFlags bind;
            
            ResolveInvokeTarget(varManager, typeFinder, info, out type, out obj, out args, out argTypesOri, out bind);
            
            List<object> argTmp = new List<object>(args);
            argTmp.RemoveAt(0);
            args = argTmp.ToArray();
            List<Type> argTypeTmp = new List<Type>(argTypesOri);
            argTypeTmp.RemoveAt(0);
            argTypesOri = argTypeTmp.ToArray();
            
            if (info.OperationTypeInfo != null)
            {
                type = typeFinder.GetType(info.OperationTypeInfo.Target);
                if (type == null)
                {
                    throw new InformationException(string.Format(CultureInfo.CurrentCulture,
                        ResourcesLocal.Instance.UnknownTypeInfoFormat, info.OperationTypeInfo.Target));
                }
            }
            
            string operation = OptimizeOperationName(type, info.Operation, args.Length);
            Type findStartType = type;

            Type[] argTypes = GetArgTypes(typeFinder, info.OperationTypeInfo, argTypesOri);

            //find operation.
            bool isObjectArrayArg = false;
            int nameMatchCount = 0;
            bool first = true;
            bool isAmbiguousArgs = false;
            while (!isAmbiguousArgs && type != null)
            {
                if (!first)
                {
                    type = type.BaseType();
                    if (type == null)
                    {
                        break;
                    }
                }
                first = false;
                
                FieldInfo field = FindField(type, bind, operation, args, ref isObjectArrayArg, ref nameMatchCount);
                if (field != null)
                {
                    return ExecuteField(async, varManager, info, obj, args, field);
                }
                
                PropertyInfo property = FindProperty(type, bind, operation, args, ref isObjectArrayArg, ref nameMatchCount);
                if (property != null)
                {
                    return ExecuteProperty(async, varManager, info, obj, args, property);
                }
                
                MethodInfo method = FindMethodOrProperty(info.OperationTypeInfo != null, type, bind, operation, argTypes,
                            ref isObjectArrayArg, ref nameMatchCount, ref isAmbiguousArgs);
                if (method != null)
                {
                    return ExecuteMethodOrProperty(async, varManager, info, obj, args, method);
                }
            }
            
            throw MakeNotFoundException(info, findStartType, argTypes, isObjectArrayArg, nameMatchCount, isAmbiguousArgs);
        }

        static FieldInfo FindField(Type type, BindingFlags bind, string operation, object[] args,
            ref bool isObjectArrayArg, ref int nameMatchCount)
        {
            FieldInfo field = type.GetField(operation, bind);
            if (field == null)
            {
                return null;
            }
            nameMatchCount++;
            if (1 < args.Length)
            {
                return null;
            }

            if (field.FieldType == typeof(object[]))
            {
                isObjectArrayArg = true;
            }
            
            if (args.Length == 1)
            {
                if (args[0] == null)
                {
                    if (!IsAssignableNull(field.FieldType))
                    {
                        return null;
                    }
                }
                else if (!field.FieldType.IsAssignableFrom(args[0].GetType()))
                {
                    return null;
                }
            }
            return field;
        }
        
        static ReturnInfo ExecuteField(IAsyncInvoke async, VarPool varManager, ProtocolInfo info,
            object obj, object[] args, FieldInfo field)
        {
            if (args.Length == 0)
            {
                VarAddress getVar = varManager.Add(null);

                KeepAlive(varManager, info.Arguments, getVar);

                async.Execute(delegate
                {
                    ReturnInfo retInfo = new ReturnInfo();
                    try
                    {
                        varManager.SetObject(getVar, field.GetValue(obj));
                    }
                    catch (Exception e)
                    {
                        retInfo = new ReturnInfo(new ExceptionInfo(e));
                    }
                    
                    varManager.SetObject((VarAddress)info.Arguments[0], retInfo);
                    
                    FreeKeepAlive(varManager, info.Arguments, getVar);
                });
                return new ReturnInfo(getVar);
            }
            else if (args.Length == 1)
            {
                KeepAlive(varManager, info.Arguments, null);

                async.Execute(delegate
                {
                    ReturnInfo retInfo = new ReturnInfo();
                    try
                    {
                        field.SetValue(obj, args[0]);
                    }
                    catch (Exception e)
                    {
                        retInfo = new ReturnInfo(new ExceptionInfo(e));
                    }

                    varManager.SetObject((VarAddress)info.Arguments[0], retInfo);

                    FreeKeepAlive(varManager, info.Arguments, null);
                });
                return new ReturnInfo();
            }
            throw new InternalException();
        }
        
        static PropertyInfo FindProperty(Type type, BindingFlags bind, string operation, object[] args,
            ref bool isObjectArrayArg, ref int nameMatchCount)
        {
            PropertyInfo field = type.GetProperty(operation, bind);
            if (field == null)
            {
                return null;
            }
            nameMatchCount++;
            if (1 < args.Length)
            {
                return null;
            }

            if (field.PropertyType == typeof(object[]))
            {
                isObjectArrayArg = true;
            }

            if (args.Length == 1)
            {
                if (args[0] == null)
                {
                    if (!IsAssignableNull(field.PropertyType))
                    {
                        return null;
                    }
                }
                else if (!field.PropertyType.IsAssignableFrom(args[0].GetType()))
                {
                    return null;
                }
            }
            return field;
        }
        
        static ReturnInfo ExecuteProperty(IAsyncInvoke async, VarPool varManager, ProtocolInfo info,
            object obj, object[] args, PropertyInfo property)
        {
            if (args.Length == 0)
            {
                VarAddress getVar = varManager.Add(null);
                
                KeepAlive(varManager, info.Arguments, getVar);
                async.Execute(delegate
                {
                    ReturnInfo retInfo = new ReturnInfo();
                    try
                    {
                        varManager.SetObject(getVar, property.GetValue(obj));
                    }
                    catch (Exception e)
                    {
                        retInfo = new ReturnInfo(new ExceptionInfo(e));
                    }
                    
                    varManager.SetObject((VarAddress)info.Arguments[0], retInfo);

                    FreeKeepAlive(varManager, info.Arguments, getVar);
                });
                return new ReturnInfo(getVar);
            }
            else if (args.Length == 1)
            {
                KeepAlive(varManager, info.Arguments, null);
                
                async.Execute(delegate
                {
                    ReturnInfo retInfo = new ReturnInfo();
                    try
                    {
                        property.SetValue(obj, args[0]);
                    }
                    catch (Exception e)
                    {
                        retInfo = new ReturnInfo(new ExceptionInfo(e));
                    }
                    
                    varManager.SetObject((VarAddress)info.Arguments[0], retInfo);
                    
                    FreeKeepAlive(varManager, info.Arguments, null);
                });
                return new ReturnInfo();
            }
            throw new InternalException();
        }
        
        static MethodInfo FindMethodOrProperty(bool isUseOperationTypeInfo, Type type, BindingFlags bind,
            string operation, Type[] argTypes, ref bool isObjectArrayArg, ref int nameMatchCount, ref bool isAmbiguousArgs)
        {
            MethodInfo[] methods = type.GetMethods(operation, bind);
            List<MethodInfo> methodList = new List<MethodInfo>();
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name != operation)
                {
                    continue;
                }

                nameMatchCount++;
                
                ParameterInfo[] paramInfos = methods[i].GetParameters();
                bool isPerfect;
                bool isObjectArrayArgTmp = false;
                if (IsMatchParameter(isUseOperationTypeInfo, argTypes, paramInfos, out isPerfect, out isObjectArrayArgTmp))
                {
                    if (isPerfect)
                    {
                        methodList.Clear();
                        methodList.Add(methods[i]);
                        break;
                    }
                    methodList.Add(methods[i]);
                }
                if (isObjectArrayArgTmp)
                {
                    isObjectArrayArg = true;
                }
            }
            
            if (methodList.Count == 1)
            {
                return methodList[0];
            }
            else if (1 < methodList.Count)
            {
                isAmbiguousArgs = true;
            }
            return null;
        }

        static ReturnInfo ExecuteMethodOrProperty(IAsyncInvoke async, VarPool varManager,
            ProtocolInfo info, object obj, object[] args, MethodInfo method)
        {
            VarAddress handle = null;
            if (method.ReturnParameter.ParameterType != typeof(void))
            {
                handle = varManager.Add(null);
            }
            
            KeepAlive(varManager, info.Arguments, handle);
            
            async.Execute(delegate
            {
                ReturnInfo retInfo = new ReturnInfo();
                try
                {
                    object retObj = method.Invoke(obj, args);
                    if (method.ReturnParameter.ParameterType != typeof(void))
                    {
                        varManager.SetObject(handle, retObj);
                    }
                    List<object> retArgsTmp = new List<object>();
                    retArgsTmp.Add(null);
                    retArgsTmp.AddRange(args);
                    ReflectArgsAfterInvoke(varManager, info.Arguments, retArgsTmp.ToArray());
                }
                catch (Exception e)
                {
                    retInfo = new ReturnInfo(new ExceptionInfo(e));
                }
                varManager.SetObject((VarAddress)info.Arguments[0], retInfo);
                
                FreeKeepAlive(varManager, info.Arguments, handle);
            });

            return new ReturnInfo(handle);
        }
        
        static InformationException MakeNotFoundException(ProtocolInfo info, Type findStartType, Type[] argTypes,
                            bool isObjectArrayArg, int nameMatchCount, bool isAmbiguousArgs)
        {
            if (isAmbiguousArgs)
            {
                throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorManyFoundInvokeFormat,
                    findStartType.Name, info.Operation, MakeErrorInvokeArgInfo(argTypes)));
            }
            else if (nameMatchCount == 0)
            {
                return new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorNotFoundInvokeFormat,
                    findStartType.Name, info.Operation, MakeErrorInvokeArgInfo(argTypes)));
            }
            else
            {
                if (isObjectArrayArg)
                {
                    return new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorArgumentInvokeFormatForObjectArray,
                        findStartType.Name, info.Operation, MakeErrorInvokeArgInfo(argTypes)));
                }
                else
                {
                    return new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorArgumentInvokeFormat,
                        findStartType.Name, info.Operation, MakeErrorInvokeArgInfo(argTypes)));
                }
            }
        }

        static void KeepAlive(VarPool varManager, object[] arguments, VarAddress handle)
        {
            if (handle != null)
            {
                varManager.KeepAlive(handle);
            }
            for (int i = 0; i < arguments.Length; i++)
            {
                VarAddress aliveHandle = arguments[i] as VarAddress;
                if (aliveHandle != null)
                {
                    varManager.KeepAlive(aliveHandle);
                }
            }
        }
        
        static void FreeKeepAlive(VarPool varManager, object[] arguments, VarAddress handle)
        {
            if (handle != null)
            {
                varManager.FreeKeepAlive(handle);
            }
            for (int i = 0; i < arguments.Length; i++)
            {
                VarAddress aliveHandle = arguments[i] as VarAddress;
                if (aliveHandle != null)
                {
                    varManager.FreeKeepAlive(aliveHandle);
                }
            }
        }
        
        static string OptimizeOperationName(Type type, string operation, int argsLength)
        {
            if (operation.IndexOf("[") != -1)
            {
                string[] a = operation.Replace("[", string.Empty).Replace("]", string.Empty).Split(new char[] { ',' }, StringSplitOptions.None);
                if (a.Length == argsLength)
                {
                    operation = type.IsArray ? "Get" : "get_Item";
                }
                else
                {
                    operation = type.IsArray ? "Set" : "set_Item";
                }
            }
            return operation;
        }
        
        static void ResolveInvokeTarget(VarPool varManager, TypeFinder typeFinder, ProtocolInfo info, out Type type, out object targetObj, out object[] args, out Type[] argTypes, out BindingFlags bind)
        {
            type = null;
            targetObj = null;
            bind = new BindingFlags();
            
            if (info.VarAddress == null)
            {
                type = typeFinder.GetType(info.TypeFullName);
                if (type == null)
                {
                    throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.UnknownTypeInfoFormat, info.TypeFullName));
                }
                bind.IsStatic = true;
            }
            else
            {
                VarAndType varAndType = varManager.GetVarAndType(info.VarAddress);
                targetObj = varAndType.Core;
                if (targetObj == null)
                {
                    throw new InformationException(ResourcesLocal.Instance.NullObjectOperation);
                }
                type = varAndType.Type;
            }
            ResolveArgs(varManager, info.Arguments, out args, out argTypes);
        }

        internal static void ResolveArgs(VarPool varManager, object[] argsInfo, out object[] args)
        {
            Type[] argTypes;
            ResolveArgs(varManager, argsInfo, out args, out argTypes);
        }

        internal static void ResolveArgs(VarPool varManager, object[] argsInfo, out object[] args, out Type[] argTypes)
        {
            args = new object[argsInfo.Length];
            argTypes = new Type[argsInfo.Length];
            for (int i = 0; i < argsInfo.Length; i++)
            {
                VarAddress handle = argsInfo[i] as VarAddress;
                
                if (handle == null)
                {
                    args[i] = argsInfo[i];
                    if (args[i] != null)
                    {
                        argTypes[i] = args[i].GetType();
                    }
                }
                else
                {
                    VarAndType varAndType = varManager.GetVarAndType(handle);
                    args[i] = varAndType.Core;
                    argTypes[i] = varAndType.Type;
                }
            }
        }

        internal static void ReflectArgsAfterInvoke(VarPool varManager, object[] argsInfo, object[] args)
        {
            if (argsInfo.Length != args.Length)
            {
                throw new InternalException();
            }
            for (int i = 0; i < argsInfo.Length; i++)
            {
                VarAddress handle = argsInfo[i] as VarAddress;
                if (handle != null)
                {
                    varManager.SetObject(handle, args[i]);
                }
            }
        }
        
        static Type[] GetArgTypes(TypeFinder typeFinder, OperationTypeInfo operationTypeInfo, Type[] argTypesOri)
        {
            List<Type> argTypes = new List<Type>();
            if (operationTypeInfo == null)
            {
                argTypes.AddRange(argTypesOri);
            }
            else
            {
                for (int i = 0; i < operationTypeInfo.Arguments.Length; i++)
                {
                    Type type = typeFinder.GetType(operationTypeInfo.Arguments[i]);
                    if (type == null)
                    {
                        throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.UnknownTypeInfoFormat, operationTypeInfo.Arguments[i]));
                    }
                    argTypes.Add(type);
                }
                
                if (operationTypeInfo.Arguments.Length == 1 &&
                    operationTypeInfo.Arguments[0] == typeof(object[]).ToString() &&
                    argTypesOri.Length != 1)
                {
                    throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorOperationTypeArgInfoForObjectArrayFormat,
                        MakeErrorInvokeArgInfo(argTypes.ToArray()), MakeErrorInvokeArgInfo(argTypesOri)));
                }

                if (argTypesOri.Length != operationTypeInfo.Arguments.Length)
                {
                    throw new InformationException(string.Format(CultureInfo.CurrentCulture, ResourcesLocal.Instance.ErrorOperationTypeArgInfoFormat,
                        MakeErrorInvokeArgInfo(argTypes.ToArray()), MakeErrorInvokeArgInfo(argTypesOri)));
                }
            }
            return argTypes.ToArray();
        }
        
        static string MakeErrorInvokeArgInfo(Type[] argTypes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Type type in argTypes)
            {
                if (0 < builder.Length)
                {
                    builder.Append(", ");
                }
                builder.Append((type == null) ? "null" : type.Name);
            }
            return builder.ToString();
        }
        
        static bool IsMatchParameter(bool isUseOperationTypeInfo, Type[] args, ParameterInfo[] paramInfos, out bool isPerfect, out bool isObjectArrayArg)
        {
            isObjectArrayArg = false;
            if (paramInfos.Length == 1)
            {
                Type paramType = (paramInfos[0].ParameterType.IsByRef) ?
                    paramInfos[0].ParameterType.GetElementType() : paramInfos[0].ParameterType;
                if (paramType == typeof(object[]))
                {
                    isObjectArrayArg = true;
                }
            }

            if (args.Length != paramInfos.Length)
            {
                isPerfect = false;
                return false;
            }
            isPerfect = true;
            for (int j = 0; j < paramInfos.Length; j++)
            {
                if (args[j] == null)
                {
                    if (!IsAssignableNull(paramInfos[j].ParameterType))
                    {
                        return false;
                    }
                    isPerfect = false;
                    continue;
                }
                
                Type paramType = (!isUseOperationTypeInfo && paramInfos[j].ParameterType.IsByRef) ?
                    paramInfos[j].ParameterType.GetElementType() : paramInfos[j].ParameterType;
                
                if (args[j] == paramType)
                {
                    continue;
                }
                
                if (isUseOperationTypeInfo && paramType.IsByRef != args[j].IsByRef)
                {
                    return false;
                }

                isPerfect = false;
                
                if (paramType.IsByRef)
                {
                    paramType = paramType.GetElementType();
                }
                
                if (!paramType.IsAssignableFrom(args[j]))
                {
                    return false;
                }
            }
            return true;
        }

        static bool IsAssignableNull(Type type)
        {
            if (!type.IsValueType())
            {
                return true;
            }
            return (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
