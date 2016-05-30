using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCS
{
    public partial class UserAuthority : Form
    {
        public RolePresenter rolePresenter = null;
        public UserPresenter userPresenter = null;
        private DataTable dt = null;
        private List<string> roleidList = null;

        public UserAuthority(List<string> roleidList)
        {
            InitializeComponent();
            rolePresenter = new RolePresenter();
            userPresenter = new UserPresenter();
            dt = new DataTable();
            this.roleidList = roleidList;
        }

        private void UserAuthority_Load(object sender, EventArgs e)
        {
            try
            {
                //选择role_function中权限
                dt = userPresenter.GetAuthListByRoleId(roleidList);
                rolePresenter.AddTree(dt,treeView1, -1, (TreeNode)null);
                treeView1.ExpandAll();//默认展开所有节点                 
            }
            catch (Exception funce)
            {
                MessageBox.Show(this.Parent, "System error" + funce.Message);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

    }
}
