using System.Collections.Generic;

namespace Friendly.Core
{
	class VarPool
	{
		UniqueNoManager _varAddressManager = new UniqueNoManager();
        Dictionary<int, VarAndType> _handleAndObject = new Dictionary<int, VarAndType>();
        Dictionary<int, int> _keepAlive = new Dictionary<int, int>();
        
        internal void KeepAlive(VarAddress varAddress)
        {
            lock (this)
            {
                int count = 0;
                if (!_keepAlive.TryGetValue(varAddress.Core, out count))
                {
                    count = 0;
                }
                count++;
                _keepAlive.Remove(varAddress.Core);
                _keepAlive.Add(varAddress.Core, count);
            }
        }

        internal void FreeKeepAlive(VarAddress varAddress)
        {
            lock (this)
            {
                int count = 0;
                if (!_keepAlive.TryGetValue(varAddress.Core, out count))
                {
                    return;
                }
                count--;
                if (count <= 0)
                {
                    _keepAlive.Remove(varAddress.Core);
                    if (!_handleAndObject.ContainsKey(varAddress.Core))
                    {
                        _varAddressManager.FreeNo(varAddress.Core);
                    }
                }
                else
                {
                    _keepAlive[varAddress.Core] = count;
                }
            }
        }
        
		internal VarAddress Add(object obj)
		{
			lock (this)
			{
				int no;
				if (!_varAddressManager.CreateNo(out no))
				{
                    throw new InformationException(ResourcesLocal.Instance.OutOfMemory);
				}
				_handleAndObject.Add(no, new VarAndType(obj));
				return new VarAddress(no);
			}
		}

        internal bool Remove(VarAddress varAddress)
		{
			lock (this)
			{
                if (!_handleAndObject.ContainsKey(varAddress.Core))
				{
					return false;
				}
                _handleAndObject.Remove(varAddress.Core);
                if (!_keepAlive.ContainsKey(varAddress.Core))
                {
                    _varAddressManager.FreeNo(varAddress.Core);
                }
				return true;
			}
		}
        
        internal VarAndType GetVarAndType(VarAddress varAddress)
        {
            lock (this)
            {
                VarAndType varAndType;
                if (_handleAndObject.TryGetValue(varAddress.Core, out varAndType))
                {
                    return new VarAndType(varAndType.Core, varAndType.Type);
                }
                throw new InternalException();
            }
        }

        internal void SetObject(VarAddress varAddress, object obj)
		{
			lock (this)
			{
                if (_handleAndObject.ContainsKey(varAddress.Core))
                {
                    _handleAndObject[varAddress.Core] = new VarAndType(obj);
                }
			}
		}
        
        internal bool IsEmptyVar(VarAddress varAddress)
        {
            lock (this)
            {
                VarAndType varAndType;
                if (_handleAndObject.TryGetValue(varAddress.Core, out varAndType))
                {
                    return object.ReferenceEquals(varAndType.Core, null);
                }
                return true;
            }
        }
    }
}
