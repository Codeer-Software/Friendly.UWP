using System.Collections.Generic;

namespace Friendly.Core
{
	public class UniqueNoManager
	{
		int _no;
		Dictionary<int, bool> _curretExistNo = new Dictionary<int, bool>();

        public bool CreateNo(out int no)
		{
			no = 0;
			_no++;
			int firstNo = _no;
			while (_no == 0 || _curretExistNo.ContainsKey(_no))
			{
				_no++;
				if (_no == firstNo)
				{
					return false;
				}
			}
			no = _no;
            _curretExistNo.Add(no, true);
			return true;
		}

        public void FreeNo(int no)
		{
			_curretExistNo.Remove(no);
		}
	}
}
