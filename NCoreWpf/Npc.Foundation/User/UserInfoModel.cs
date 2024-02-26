using Newtonsoft.Json;
using Npc.Foundation.Base;
using Npc.Foundation.Define;
using Npc.Foundation.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Npc.Foundation.User
{
    /// <summary>
    /// UserInfoModel Class
    /// </summary>
    public class UserInfoModel
    {
        /// <summary>
        /// User List
        /// </summary>
        public ObservableCollection<UserModel> Users { get; set; } = new ObservableCollection<UserModel>();

        /// <summary>
        /// Recent User List
        /// </summary>
        public List<RecentUserModel> RecentUsers { get; set; } = new List<RecentUserModel>();

        /// <summary>
        /// UserLevel List
        /// </summary>
        [JsonIgnore]
        public List<UserLevelModel> UserLevels { get; set; } = new List<UserLevelModel>();
    }

    /// <summary>
    /// UserModel Class
    /// </summary>
    public class UserModel : ModelBase
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string ID
        {
            get { return _id; }
            set
            {
                if (String.Equals(_id, value) == false)
                {
                    _id = value;
                    RaisePropertyChanged();
                }
            }
        }
        private String _id = String.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string PASSWD
        {
            get { return _passwd; }
            set
            {
                if (String.Equals(_passwd, value) == false)
                {
                    _passwd = value;
                    RaisePropertyChanged();
                }
            }
        }
        private String _passwd = String.Empty;

        /// <summary>
        /// Barcode
        /// </summary>
        public string BARCODE
        {
            get { return _barcode; }
            set
            {
                if (String.Equals(_barcode, value) == false)
                {
                    _barcode = value;
                    RaisePropertyChanged();
                }
            }
        }
        private String _barcode = String.Empty;

        /// <summary>
        /// User Level
        /// </summary>
        public string USER_LEVEL
        {
            get { return _userLevel; }
            set
            {
                if (String.Equals(_userLevel, value) == false)
                {
                    _userLevel = value;
                    RaisePropertyChanged();
                }
            }
        }
        private String _userLevel = String.Empty;

        /// <summary>
        /// Group Name
        /// </summary>
        public string GROUP_NAME
        {
            get { return _groupName; }
            set
            {
                if (String.Equals(_groupName, value) == false)
                {
                    _groupName = value;
                    RaisePropertyChanged();
                }
            }
        }
        private String _groupName = String.Empty;

        /// <summary>
        /// User Name
        /// </summary>
        public string USER_NAME
        {
            get { return _userName; }
            set
            {
                if (String.Equals(_userName, value) == false)
                {
                    _userName = value;
                    RaisePropertyChanged();
                }
            }
        }
        private String _userName = String.Empty;

        /// <summary>
        /// Last access time
        /// </summary>
        public DateTime? LAST_ACCESS_TIME
        {
            get { return _lastAccessTime; }
            set
            {
                if (DateTime.Equals(_lastAccessTime, value) == false)
                {
                    _lastAccessTime = value;
                    RaisePropertyChanged();
                }
            }
        }
        private DateTime? _lastAccessTime = null;

        [JsonIgnore]
        /// <summary>
        /// 수정이나 삭제 가능 여부
        /// </summary>
        public Boolean Editable
        {
            get { return _editable; }
            set
            {
                if (_editable != value)
                {
                    _editable = value;
                    RaisePropertyChanged();
                }
            }
        }
        private Boolean _editable = false;

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public UserModel Clone()
        {
            UserModel newUser = new UserModel();
            newUser.ID = this.ID;
            newUser.PASSWD = this.PASSWD;
            newUser.BARCODE = this.BARCODE;
            newUser.USER_LEVEL = this.USER_LEVEL;
            newUser.USER_NAME = this.USER_NAME;
            newUser.LAST_ACCESS_TIME = this.LAST_ACCESS_TIME;
            newUser.Editable = this.Editable;

            return newUser;
        }
    }

    /// <summary>
    /// LoginUserModel Class
    /// </summary>
    public class LoginUserModel : ModelBase
    {
        private string _id;
        /// <summary>
        /// User ID
        /// </summary>
        public string ID
        {
            get { return this._id; }
            set
            {
                this._id = value;
                this.RaisePropertyChanged();
            }
        }

        private string _password;
        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get { return this._password; }
            set
            {
                this._password = value;
                this.RaisePropertyChanged();
            }
        }

        private string _barcode;
        /// <summary>
        /// Barcode
        /// </summary>
        public string Barcode
        {
            get { return this._barcode; }
            set
            {
                this._barcode = value;
                this.RaisePropertyChanged();
            }
        }

        private int _userLevel;
        /// <summary>
        /// UserLevel
        /// </summary>
        public int UserLevel
        {
            get { return this._userLevel; }
            set
            {
                this._userLevel = value;
                this.RaisePropertyChanged();
            }
        }

        private string _userLevelName;
        /// <summary>
        /// UserLevel Name
        /// </summary>
        public string UserLevelName
        {
            get { return this._userLevelName; }
            set
            {
                this._userLevelName = value;
                this.RaisePropertyChanged();
            }
        }

        private string _userLevelDisplayName;
        /// <summary>
        /// UserLevel DisplayName
        /// </summary>
        public string UserLevelDisplayName
        {
            get { return this._userLevelDisplayName; }
            set
            {
                this._userLevelDisplayName = value;
                this.RaisePropertyChanged();
            }
        }

        private string _groupName;
        /// <summary>
        /// Group Name
        /// </summary>
        public string GroupName
        {
            get { return this._groupName; }
            set
            {
                this._groupName = value;
                this.RaisePropertyChanged();
            }
        }

        private string _name;
        /// <summary>
        /// UserName
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                this.RaisePropertyChanged();
            }
        }

        private string _initial;
        /// <summary>
        /// UserName Initial
        /// </summary>
        public string Initial
        {
            get { return this._initial; }
            set
            {
                this._initial = value;
                this.RaisePropertyChanged();
            }
        }

        private int _currentUserLevel;
        /// <summary>
        /// Current UserLevel
        /// </summary>
        public int CurrentUserLevel
        {
            get { return this._currentUserLevel; }
            set
            {
                this._currentUserLevel = value;
                this.RaisePropertyChanged();
            }
        }

        private string _currentUserLevelName;
        /// <summary>
        /// Current UserLevel Name
        /// </summary>
        public string CurrentUserLevelName
        {
            get { return this._currentUserLevelName; }
            set
            {
                this._currentUserLevelName = value;
                this.RaisePropertyChanged();
            }
        }

        private string _currentUserLevelDisplayName;
        /// <summary>
        /// Current UserLevel DisplayName
        /// </summary>
        public string CurrentUserLevelDisplayName
        {
            get { return this._currentUserLevelDisplayName; }
            set
            {
                this._currentUserLevelDisplayName = value;
                this.RaisePropertyChanged();
            }
        }

        private bool _isLogin = false;
        /// <summary>
        /// IsLogin
        /// </summary>
        public bool IsLogin
        {
            get { return this._isLogin; }
            set
            {
                this._isLogin = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// UserLevel 변수를 UserLevelTypes Enum으로 변환하여 가져온다.
        /// </summary>
        /// <returns></returns>
        public UserLevelTypes GetUserLevel()
        {
            return TypeConvertUtil.EnumConverter<UserLevelTypes>(this.UserLevel);
        }
    }

    /// <summary>
    /// RecentUserModel Class
    /// </summary>
    public class RecentUserModel
    {
        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Login DateTime
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string ID { get; set; }
    }

    /// <summary>
    /// UserLevelModel
    /// </summary>
    public class UserLevelModel
    {
        /// <summary>
        /// Level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Level Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Level DisplayName
        /// </summary>
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// FrameworkElement Authorization Model Class
    /// </summary>
    public class UIAuthorizationModel
    {
        /// <summary>
        /// UI Guid (View Instance Unique Key)
        /// </summary>
        public Guid UIGuid { get; set; }

        /// <summary>
        /// View FullName
        /// </summary>
        public string ViewFullName { get; set; }

        /// <summary>
        /// FrameworkElement
        /// </summary>
        public FrameworkElement ElementObject { get; set; }

        /// <summary>
        /// Setting by authority
        /// </summary>
        public List<AuthTypeModel> AuthTypes { get; set; } = new List<AuthTypeModel>();

        /// <summary>
        /// Clone AuthTypes
        /// </summary>
        /// <returns></returns>
        public List<AuthTypeModel> CloneAuthTypes()
        {
            List<AuthTypeModel> authTypes = new List<AuthTypeModel>();
            foreach (var authType in this.AuthTypes)
            {
                authTypes.Add(authType.Clone());
            }

            return authTypes;
        }
    }

    /// <summary>
    /// Auth Type Model Class
    /// </summary>
    public class AuthTypeModel
    {
        /// <summary>
        /// UserLevel
        /// </summary>
        public UserLevelTypes UserLevel { get; set; }

        /// <summary>
        /// IsEnabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Visibility
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userLevel"></param>
        /// <param name="isEnabled"></param>
        /// <param name="visibility"></param>
        public AuthTypeModel(UserLevelTypes userLevel, bool isEnabled, Visibility visibility)
        {
            this.UserLevel = userLevel;
            this.IsEnabled = isEnabled;
            this.Visibility = visibility;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public AuthTypeModel Clone()
        {
            return new AuthTypeModel(this.UserLevel, this.IsEnabled, this.Visibility);
        }
    }
}
