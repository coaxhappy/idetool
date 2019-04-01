using GalaSoft.MvvmLight;
using Hami.WPF.IDETool.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hami.WPF.IDETool.ViewModel
{
    public class BaseViewModel : ViewModelBase
    {
        private string message = string.Empty;

        public string Message
        {
            get { return message; }
            set
            {
                if (value != message)
                {
                    message = value;
                    RaisePropertyChanged(() => this.Message);
                }
            }
        }

        private MessageType msgType;

        public MessageType MsgType
        {
            get { return msgType; }
            set
            {
                if (value != msgType)
                {
                    msgType = value;
                    RaisePropertyChanged(() => this.MsgType);
                }
            }
        }

        private bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (value != isBusy)
                {
                    isBusy = value;
                    RaisePropertyChanged(() => this.IsBusy);
                }
            }
        }

        private string busyContent;

        public string BusyContent
        {
            get { return busyContent; }
            set
            {
                if (value != busyContent)
                {
                    busyContent = value;
                    RaisePropertyChanged(() => this.BusyContent);
                }
            }
        }

        #region Protected Methods
        public void ShowMessage(string message, MessageType msgType)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Message = message;
                MsgType = msgType;
            });
        }
        #endregion
    }
}
