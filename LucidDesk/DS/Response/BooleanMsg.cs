using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LucidDesk.DS.Response
{

    [Serializable]
    public struct BooleanMsg<T>
    {
        private string message;

        private string trace;

        [XmlIgnore]
        public Exception ExceptionInfo { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Message
        {
            get
            {
                return message ?? "";
            }
            set
            {
                message = value;
            }
        }

        [XmlAttribute]
        public bool Result { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Trace
        {
            get
            {
                return trace ?? "";
            }
            set
            {
                trace = value;
            }
        }
        [XmlElement]
        public T Value { get; set; }

        public BooleanMsg(string msg = "")
        {
            message = msg;
            trace = "";
            Result = string.IsNullOrEmpty(msg);
            Value = default(T);
            ExceptionInfo = null;
        }

        public BooleanMsg(bool result = true, string msg = "")
        {
            Result = result;
            message = msg;
            trace = string.Empty;
            Value = default(T);
            ExceptionInfo = null;
        }
        public BooleanMsg(T value, bool result = true, string msg = "")
        {
            Result = result;
            message = msg;
            trace = string.Empty;
            Value = value;
            ExceptionInfo = null;
        }
        public BooleanMsg(Exception e, T value = default(T))
        {
            ExceptionInfo = e;
            message = e.Message;
            trace = e.StackTrace;
            Value = value;
            Result = false;
        }
        public bool Equals(BooleanMsg<T> other)
        {
            return string.Equals(message, other.message) && Result == other.Result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is BooleanMsg<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ((message?.GetHashCode() ?? 0) * 397) ^ Result.GetHashCode();
        }

        public static implicit operator BooleanMsg<T>(string value)
        {
            return new BooleanMsg<T>(value);
        }
        public static implicit operator BooleanMsg<T>(bool value)
        {
            return new BooleanMsg<T>(value);
        }
        public static implicit operator BooleanMsg<T>(T value)
        {
            return new BooleanMsg<T>(value);
        }
        public static implicit operator string(BooleanMsg<T> value)
        {
            return value.Message;
        }
        public static implicit operator T(BooleanMsg<T> value)
        {
            return value.Value;
        }

        public static implicit operator bool(BooleanMsg<T> value)
        {
            return value.Result;
        }

        public static bool operator ==(BooleanMsg<T> x, BooleanMsg<T> y)
        {
            return x.Result == y.Result;
        }
        public static string operator +(BooleanMsg<T> x, BooleanMsg<T> y)
        {
            return x.ToString() + y.ToString();
        }
        public static bool operator !=(BooleanMsg<T> x, BooleanMsg<T> y)
        {
            return x.Result != y.Result;
        }
        public override string ToString()
        {
            return Message;
        }

        public BooleanMsg<T> OnFailure(Action<BooleanMsg<T>> resultMethod)
        {
            if (!Result)
            {
                resultMethod(this);
            }
            return this;
        }
        public BooleanMsg<T> OnSuccess(Action<BooleanMsg<T>> resultMethod)
        {
            if (Result)
            {
                resultMethod(this);
            }
            return this;
        }
        public BooleanMsg<T> OnResultNull(Action<BooleanMsg<T>> resultMethod)
        {
            if (Value == null)
            {
                resultMethod(this);
            }

            return this;
        }

        public BooleanMsg<T> OnResultNotNull(Action<BooleanMsg<T>> resultMethod)
        {
            if (Value != null)
            {
                resultMethod(this);
            }

            return this;
        }

        public BooleanMsg<T> OnResult(Action<BooleanMsg<T>> resultMethod)
        {
            resultMethod(this);
            return this;
        }
    }


    [Serializable]
    public struct BooleanMsg
    {
        private string message;

        private string trace;

        [XmlIgnore]
        public Exception ExceptionInfo { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Message
        {
            get
            {
                return message ?? "";
            }
            set
            {
                message = value;
            }
        }

        [XmlAttribute]
        public bool Result { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Trace
        {
            get
            {
                return trace ?? "";
            }
            set
            {
                trace = value;
            }
        }

        public BooleanMsg(string msg = "")
        {
            message = msg;
            Result = string.IsNullOrEmpty(msg);
            trace = "";
            ExceptionInfo = null;
        }

        public BooleanMsg(bool result = true, string msg = "")
        {
            Result = result;
            message = msg;
            trace = "";
            ExceptionInfo = null;
        }

        public BooleanMsg(Exception e)
        {
            Result = false;
            message = e.Message;
            trace = e.StackTrace;
            ExceptionInfo = e;
        }

        public bool Equals(BooleanMsg other)
        {
            return string.Equals(message, other.message) && Result == other.Result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is BooleanMsg other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ((message?.GetHashCode() ?? 0) * 397) ^ Result.GetHashCode();
        }

        public static implicit operator bool(BooleanMsg value)
        {
            return value.Result;
        }

        public static implicit operator BooleanMsg(string value)
        {
            return new BooleanMsg(value);
        }

        public static implicit operator BooleanMsg(bool value)
        {
            return new BooleanMsg(value);
        }

        public static implicit operator string(BooleanMsg value)
        {
            return value.Message;
        }

        public static bool operator ==(BooleanMsg x, BooleanMsg y)
        {
            return x.Result == y.Result;
        }

        public static string operator +(BooleanMsg x, BooleanMsg y)
        {
            return x.ToString() + y.ToString();
        }

        public static bool operator !=(BooleanMsg x, BooleanMsg y)
        {
            return x.Result != y.Result;
        }

        public BooleanMsg OnFailure(Action<BooleanMsg> resultMethod)
        {
            if (!Result)
            {
                resultMethod(this);
            }

            return this;
        }

        public BooleanMsg OnSuccess(Action<BooleanMsg> resultMethod)
        {
            if (Result)
            {
                resultMethod(this);
            }

            return this;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}

