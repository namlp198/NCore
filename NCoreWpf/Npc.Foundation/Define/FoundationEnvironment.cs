using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Define
{
    /// <summary>
    /// Foundation 환경 변수
    /// </summary>
    public class FoundationEnvironment
    {
        #region Singleton
        private static readonly FoundationEnvironment _instance = new FoundationEnvironment();
        /// <summary>
        /// CommonEnvironment Instance
        /// </summary>
        public static FoundationEnvironment Instance { get { return _instance; } }
        #endregion Singleton

        /// <summary>
        /// Appliction Type
        /// </summary>
        public ApplicationTypes ApplicationType = ApplicationTypes.Vision;

        /// <summary>
        /// LogView Popup 사용여부
        /// </summary>
        public bool IsShowLogView { get; set; }

        /// <summary>
        /// 시작 경로 설정
        /// </summary>
        /// <param name="dir"></param>
        public void SetApplicationType(ApplicationTypes applicationType)
        {
            this.ApplicationType = applicationType;
        }

        /// <summary>
        /// Application Startup Directory Path
        /// </summary>
        public string StartupDir { get; private set; }

        /// <summary>
        /// 시작 경로 설정
        /// </summary>
        /// <param name="dir"></param>
        public void SetStartupDirectory(string dir)
        {
            this.StartupDir = dir;
        }
    }
}
