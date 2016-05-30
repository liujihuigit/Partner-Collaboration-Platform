using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaCS
{
    public class Role
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
        private string createdate;

        public string Createdate
        {
            get { return createdate; }
            set { createdate = value; }
        }
        private string createtime;

        public string Createtime
        {
            get { return createtime; }
            set { createtime = value; }
        }
        private string createuser;

        public string Createuser
        {
            get { return createuser; }
            set { createuser = value; }
        }
        private string updatedate;

        public string Updatedate
        {
            get { return updatedate; }
            set { updatedate = value; }
        }
        private string updatetime;

        public string Updatetime
        {
            get { return updatetime; }
            set { updatetime = value; }
        }
        private string updateuser;

        public string Updateuser
        {
            get { return updateuser; }
            set { updateuser = value; }
        }
    }
}
