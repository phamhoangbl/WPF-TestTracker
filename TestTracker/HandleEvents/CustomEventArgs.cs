using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTracker.HandleEvents
{
    public class TextArgs : EventArgs
    {
        #region Fields
        private string szMessage;
        #endregion Fields

        #region ConstructorsH
        public TextArgs(string TextMessage)
        {
            szMessage = TextMessage;
        }
        #endregion Constructors

        #region Properties
        public string Message
        {
            get { return szMessage; }
            set { szMessage = value; }
        }
        #endregion Properties
    }

    public class MessageArgs : EventArgs
    {
        #region Fields
        private bool _isSuccess;
        private string _message;
        #endregion Fields

        #region ConstructorsH
        public MessageArgs(string message, bool isSuccess)
        {
            _message = message;
            _isSuccess = isSuccess;
        }
        #endregion Constructors

        #region Properties
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        public bool IsSuccess
        {
            get { return _isSuccess; }
            set { _isSuccess = value; }
        }
        #endregion Properties
    }
    public class InVoke : EventArgs
    {
        #region Fields
        private bool _isSuccess;
        #endregion Fields

        #region ConstructorsH
        public InVoke(bool isSuccess)
        {
            _isSuccess = isSuccess;
        }
        #endregion Constructors

        #region Properties
        public bool IsSuccess
        {
            get { return _isSuccess; }
            set { _isSuccess = value; }
        }
        #endregion Properties
    }
}
