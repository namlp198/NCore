using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;

namespace ReadCodeGUI.Models
{
    public class ExcelTemplateModel : ModelBase
    {
        private int m_nId;
        private string m_strProductName;
        private string m_strProductCode;
        private string m_strDate;
        private string m_strJudgement;
        private string m_strNote;

        public int Id { get { return m_nId; } set { if (SetProperty(ref m_nId, value)) { } } }
        public string ProductName { get { return m_strProductName; } set { if (SetProperty(ref m_strProductName, value)) { } } }
        public string ProductCode { get { return m_strProductCode; } set { if (SetProperty(ref m_strProductCode, value)) { } } }
        public string Date { get { return m_strDate; } set { if (SetProperty(ref m_strDate, value)) { } } }
        public string Judgement { get { return m_strJudgement; } set { if (SetProperty(ref m_strJudgement, value)) { } } }
        public string Note { get { return m_strNote; } set { if (SetProperty(ref m_strNote, value)) { } } }
    }
}
