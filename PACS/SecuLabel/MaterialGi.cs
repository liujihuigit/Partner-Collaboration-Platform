using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PaCSTools;
using DevExpress.XtraSplashScreen;
using DevExpress.Data;
using System.Data.OracleClient;

namespace SecuLabel
{
    public partial class MaterialGi : DevExpress.XtraEditors.XtraForm
    {
        string ls_sdate, ls_edate;
        string ls_plant;
        string plantInsert;
        string vend4;
        int iCount = 0;

        string type = ""; //移库类型

        //取消入库变量
        string ls_bk_company = "";



        //
        string ls_remark, ls_material_code, ls_doc_type, ls_doc_mvt, ls_move_company, ls_move_line,barCode, prodLotNo="";
        string ls_prod_plan_date = "";
        int ll_doc_qty;

        string bk_docno;


        public MaterialGi()
        {
            InitializeComponent();
        }


        private void MeterialGi_Load(object sender, EventArgs e)
        {
            SecuGlobal.setDate(dateEditFrom, dateEditTo);


            if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
                cbPlant.Text = "SSDP";
            else
                cbPlant.Text = "SESC";

            plantInsert = cbPlant.Text;

            string bufVend = SecuGlobal.getPopVendorInfo(PaCSGlobal.LoginUserInfo.Venderid, PaCSGlobal.LoginUserInfo.Fct_code);
            if (!bufVend.Equals(""))
            {
                cbVendor.Text = bufVend;
            }
            else
            {
                cbVendor.Text = PaCSGlobal.LoginUserInfo.Venderid + ":" + PaCSGlobal.LoginUserInfo.Vendername;  //苏州法人
            }

            if (!bGetInfo(bufVend.Substring(0,4)))
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "材料信息/line 信息获取失败");
                return;
            }

            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
        }




        private bool bGetInfo(string vendcode4)
        {
            try
            {
                string sql = "select code , code_name from " + SecuGlobal.tb_code + " where prefix = '330' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";  //移库类型
                OracleDataReader odr = OracleHelper.ExecuteReader(sql);
                if (odr.HasRows)
                {
                    cbDocMvt.Properties.Items.Clear();
                    while (odr.Read())
                    {
                        cbDocMvt.Properties.Items.Add(odr["code"].ToString().Trim() + ":" + odr["code_name"].ToString().Trim());
                    }
                }
                else
                {
                    return false ;
                }

           
                string sql1 = "  select b.deptname||' - '||a.deptname dept_name, " +
                                "        a.deptid dept_id       " +
                                " from " + SecuGlobal.mv_ep_dept + " a,mv_ep_dept b     " +
                                " where a.dept_upper = b.deptid(+)   " +
                                " and a.levelid = 'L5'               " +
                                " and a.useflag = 'Y'  " +
                                " and a.vendorcode = '" + vendcode4 + "' " +
                                " order by 1,2 ";

                OracleDataReader odr1 = OracleHelper.ExecuteReader(sql1);  // LINE 信息
                if (odr1.HasRows)
                {
                    cbMoveLine.Properties.Items.Clear();
                    while (odr1.Read())
                    {
                        cbMoveLine.Properties.Items.Add(odr1["dept_name"].ToString().Trim() + ":" + odr1["dept_id"].ToString().Trim());
                    }
                }
                else
                {
                    cbMoveLine.Properties.Items.Add("SSDP:No_Line_Info");
                }


                string sql2 = " SELECT TRIM(material_code) FROM " + SecuGlobal.tbMaster + " where FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  ORDER BY material_code ";
                OracleDataReader odr2 = OracleHelper.ExecuteReader(sql2);

                if (odr2.HasRows)
                {
                    cbMaterial.Properties.Items.Clear();

                    while (odr2.Read())
                    {
                        cbMaterial.Properties.Items.Add(odr2["TRIM(material_code)"]);
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch(Exception )
            {
                return false;
            }
        }



        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正在查询数据，请稍等...");
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));

            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message);
            }
        }



        /// <summary>
        /// 获取SQL 查询的变量信息
        /// 加载COMBOX 的基本信息
        /// </summary>
        private void getVariable()
        {
            vend4 = cbVendor.Text.Trim();
            if (!string.IsNullOrEmpty(vend4))
            {
                if (!vend4.Equals("ALL"))
                {
                    string[] split = vend4.Split(new Char[] { ':' });
                    vend4 = split[0].Trim();
                }
                else
                {
                    vend4 = "";
                }
            }


            ls_plant = cbPlant.Text.Trim();
            if (!ls_plant.Equals("ALL"))
                    ls_plant = ls_plant + "%";
            else
                    ls_plant = "%";


            ls_sdate = dateEditFrom.Text.Trim().Replace("-", "");
            ls_edate = dateEditTo.Text.Trim().Replace("-", "");
        }



        /// <summary>
        /// 查询厂家申请单信息：TB_SECURITY_DOC
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="vendor"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <returns></returns>
        private string getSql(string plant,string vendor, string sDate, string eDate )
        {
            string sql =    " SELECT FCT_CODE,COMPANY, DOC_NO, DOC_SEQ, DOC_TYPE, DOC_MVT, MATERIAL_CODE,  " +
                            "        (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where a.material_code = b.matnr) description, " +
                            "        DOC_QTY,  MOVE_COMPANY, MOVE_LINE, REQ_DOC, REQ_SEQ, DOC_DATE, DOC_TIME, DOC_USER,  " +
                            "        DOC_IP, BK_COMPANY, BK_DOC_NO, BK_DOC_SEQ, BARCODE_FLAG, BOXNO, ROLLNO, LOTNO,  " +
                            "        SN_FROM, SN_TO, PROD_LOTNO, REMARK,  " +
                            "        UPDATE_DATE, UPDATE_TIME,  UPDATE_USER, UPDATE_IP, prod_plan_date, plant " +
                            " FROM " + SecuGlobal.tbSecurityDoc  + " a " +
                            " WHERE DOC_DATE BETWEEN '" + sDate + "' AND '" + eDate + "' " +
                            " AND COMPANY = '" + vendor + "' " +
                            " and a.plant like '" + plant + "' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                            " ORDER BY DOC_TYPE,DOC_MVT,DOC_DATE DESC,DOC_TIME DESC ";
            return sql;
        }




        /// <summary>
        /// 更改标题信息显示到GRIDVIEW
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {
            string[] colField = { "FCT_CODE", "plant", "COMPANY", "DOC_NO", "DOC_SEQ", "DOC_MVT", "DOC_TYPE", "MATERIAL_CODE",
                                    "description","DOC_QTY" ,"MOVE_COMPANY","MOVE_LINE","REQ_DOC","REQ_SEQ","prod_plan_date","DOC_DATE","DOC_TIME","DOC_USER",
                                    "DOC_IP","BK_COMPANY","BK_DOC_NO","BK_DOC_SEQ","BARCODE_FLAG","BOXNO","ROLLNO","LOTNO",
                                    "SN_FROM","SN_TO","PROD_LOTNO","REMARK","UPDATE_DATE","UPDATE_TIME","UPDATE_USER","UPDATE_IP"};

            string[] colName = {  "Fct Code","Plant", "厂家", "申请单号", "序号", "移库方向", "移库类型", "材料", 
                                 "材料描述","数量" ,"接收厂家","接收LINE","Req Doc","Req Seq","计划日期","Doc Date","Doc Time","Doc User",
                                    "DOC_IP","BK_COMPANY","BK_DOC_NO","BK_DOC_SEQ","条形码","BOXNO","ROLLNO","LOTNO",
                                    "SN_FROM","SN_TO","Prod LotNo","备注","更新日期","更新时间","更新人","更新IP"};

            for (int i = 0; i < colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);
            }

            return dt;
        }




        /// <summary>
        /// 后台查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    getVariable(); //获取变量信息
                    if (vend4.Equals(""))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "vendor code 4 为空");
                        return;
                    }

                    SecuGlobal.GridViewInitial(gridView1, gridControl1);
 
                    DataTable dt = OracleHelper.ExecuteDataTable(getSql(ls_plant, vend4, ls_sdate, ls_edate));  //ls_plant - %
         
                    if (dt == null)
                    {
                        SecuGlobal.showOK(panelStatus, lblStatus, "没有查询到符合条件的数据");
                        return;
                    }

                    dt = setDtHeader(dt); //更改标题栏和显示顺序
                    gridControl1.DataSource = dt;
                    gridView1.BestFitColumns();

                    gridView1.Columns["申请单号"].SummaryItem.SummaryType = SummaryItemType.Count;
                    gridView1.Columns["申请单号"].SummaryItem.DisplayFormat = "All:{0:f0} ";

                    GridCheckMarksSelection selection = new GridCheckMarksSelection(gridView1); // 增加CHECKBOX 
                    selection.CheckMarkColumn.VisibleIndex = 0;
                    selection.SelectionChanged += selection_SelectionChanged;

                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                });
            }
            catch (Exception err)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
            }
        }



        /// <summary>
        /// 选中对应申请单之后，数据显示到控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selection_SelectionChanged(object sender, EventArgs e)
        {
            iCount = 0;
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if ((bool)gridView1.GetRowCellValue(i, gridView1.Columns[0]))
                {
                    type = gridView1.GetRowCellValue(i, gridView1.Columns["移库类型"]).ToString();
                    string assy = gridView1.GetRowCellValue(i, gridView1.Columns["材料"]).ToString();
                    cbMaterial.Text = assy;
                    tbDesc.Text = gridView1.GetRowCellValue(i, gridView1.Columns["材料描述"]).ToString();

                    barCode = gridView1.GetRowCellValue(i, gridView1.Columns["条形码"]).ToString();
                    string sql = "select nvl(sum(STOCK_QTY),0) stock_qty from " + SecuGlobal.tbSecurityStock + " where " +
                                 "material_code = '" + assy + "' AND company = '" + vend4 + "' AND PLANT = '" + plantInsert + "' and " +
                                 "FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'"; //plantInsert -SSDP OR SESC
                    tbStockQty.Text = OracleHelper.ExecuteScalar(sql).ToString();
                    SecuGlobal.showOK(panelStatus, lblStatus, "选中材料是：" + assy);

                    if (ckCancle.Checked)  // 出库取消模式下，不是NOR 的不允许取消
                    {
                        string mov = gridView1.GetRowCellValue(i, gridView1.Columns["移库方向"]).ToString();
                        if (!mov.Equals("NOR") || type.Equals("GR"))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "此记录不能做出库取消，请再次确认");
                            gridView1.SetRowCellValue(i, gridView1.Columns[0], false);
                            return;
                        }
                        else
                        {
                            btnSave.Enabled = true;
                            bk_docno = gridView1.GetRowCellValue(i, gridView1.Columns["申请单号"]).ToString();  //RTN 记录单号
                            ls_bk_company = gridView1.GetRowCellValue(i, gridView1.Columns["厂家"]).ToString();  //RTN 厂家信息

                            if (!bCheckCancleQty(ls_bk_company, bk_docno, "1"))
                            {
                                SecuGlobal.showNG(panelStatus, lblStatus, "数据取消分析NG，请在确认");
                                btnSave.Enabled = false;
                                return;
                            }
                            SecuGlobal.showOK(panelStatus, lblStatus, "数据可以取消，请继续...");
                        }
                    }

                    iCount += 1;
                    
                    break;
                }
            }
        }



        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }


        /// <summary>
        /// 出库取消操作判定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool bCheckCancleQty(string ls_company, string ls_doc_no, string li_doc_seq)
        {
            try
            {
                int ll_doc_qty = 0, ll_have_bk_doc_qty = 0;
                string sql = "select nvl(doc_qty,0) from " + SecuGlobal.tbSecurityDoc + " where " +
                    "company = '" + ls_company + "' and doc_no = '" + ls_doc_no + "' and doc_seq = '" + li_doc_seq + "' " +
                    "and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'";

                ll_doc_qty = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql).ToString());

                string sql1 = "select nvl(sum(doc_qty),0) ll_have_bk_doc_qty  from " + SecuGlobal.tbSecurityDoc + " where " +
                                "bk_company = '" + ls_company + "' and bk_doc_no = '" + ls_doc_no + "' and bk_doc_seq = '" + li_doc_seq + "' " +
                                "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'";
                ll_have_bk_doc_qty = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql1).ToString());

                if (ll_have_bk_doc_qty == ll_doc_qty)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "本条记录已全部返退,不能再进行返退!");
                    return false; ;
                }

                tbDocQty.Text = (ll_doc_qty - ll_have_bk_doc_qty).ToString(); //可以返退的数量

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




        /// <summary>
        /// 选择出库类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDocMvt_SelectedIndexChanged(object sender, EventArgs e)
        {
            string bufType = "";
            cbMoveCompany.Enabled = false ;
            cbMoveLine.Enabled = false;
            if (cbDocMvt.SelectedIndex == -1)
                return;

            string buf = cbDocMvt.Text.Trim();
            string[] split = buf.Split(new Char[] { ':' });
            string code = split[0].Trim();
            string codeDesc = split[1].Trim();

            switch (code)
            {
                case "TRS":
                    bufType = "出库";
                    ls_doc_type = "GI";
                    cbMoveCompany.Enabled = true;
                    break;
                case "DEF":
                    bufType = "出库";
                    ls_doc_type = "GI";
                    break ;
                case "LOS":
                    bufType = "出库";
                    ls_doc_type = "GI";
                    cbMoveLine.Enabled = true ;
                    break;
                case "NOR":
                    bufType = "出库";
                    ls_doc_type = "GI";
                    cbMoveLine.Enabled = true;
                    break;
                case "INI":
                    ls_doc_type = "INI";
                    break;
                case "RT1":
                    bufType = "入库";
                    ls_doc_type = "GR";
                    break;
                case "RT2":
                    bufType = "入库";
                    ls_doc_type = "GR";
                    break;
                case "RT3":
                    bufType = "入库";
                    ls_doc_type = "GR";
                    break;
                case "RTN":
                    bufType = "入库";
                    ls_doc_type = "GR";
                    break;

            }

            //cbDocMvt.Text = codeDesc;
            tbDocType.Text = bufType;
            //cbMoveLine.Properties.Items.Clear();
        }





        /// <summary>
        /// 获取基本的变量信息
        /// </summary>
        /// <returns></returns>
        private bool  bGetValues()
        {
            try
            {
                ls_material_code = cbMaterial.Text.Trim();
                ll_doc_qty = System.Convert.ToInt32(tbDocQty.Text.Trim());  //出库数量
                ls_move_line = cbMoveLine.Text.Trim();   //出库到那个LINE - LINE信息需要截取
                ls_remark = tbRemark.Text.Trim();

                string buf = cbDocMvt.Text.Trim();
                if (buf.Contains(":"))
                {
                    string[] split = buf.Split(new Char[] { ':' });
                    ls_doc_mvt = split[0].Trim();
                }
                else
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择类型！");
                    return false;
                }


                if (ll_doc_qty <= 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请输入出库数量！");
                    return false;
                }



                if (ls_doc_mvt.Equals("LOS") || ls_doc_mvt.Equals("NOR"))  //正常出库模式下必须选择计划日期
                {
                    if (string.IsNullOrEmpty(deProdPlanDate.Text.Trim()))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请选择计划日期");
                        return false;
                    }
                    else
                    {
                        ls_prod_plan_date = deProdPlanDate.Text.Replace("-", "");  //计划日期
                    }
                }

                if ( ls_doc_mvt.Equals("NOR"))
                {
                    if (string.IsNullOrEmpty(ls_move_line))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请选择生产线信息");
                        return false;
                    }
                }


                if (string.IsNullOrEmpty(ls_doc_type) || ls_doc_type.Equals("INI"))   
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "不能选择初始类型！");
                    return false;
                }

                ls_move_company = cbMoveCompany.Text.Trim(); // 格式是： C660：SSDP
                if(ls_move_company.Contains(":"))
                {
                    ls_move_company = ls_move_company.Substring(0,4);  
                }

                if (ls_doc_type.Equals("GI"))
                {
                    if (ll_doc_qty > System.Convert.ToInt32(tbStockQty.Text.Trim()))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "输入的数量大于库存数量");
                        return false;
                    }
                }


                prodLotNo = tbProdLotNo.Text;  //DB 中可以为空，所以，不做判断

                return true;
            }
            catch (Exception bGetValues)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, bGetValues.Message );
                return false; 
            }
        }




        /// <summary>
        /// 判断必要条件
        /// </summary>
        /// <returns></returns>
        private bool bAnalysis()
        {
            try
            {
                if (ls_doc_mvt.Equals("INI"))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "出库操作不允许选择<期初库存>类型!");
                    return false ;
                }

                if (string.IsNullOrEmpty(ls_doc_mvt) || ll_doc_qty == 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "出库类型和出库数量是必填项");
                    return false;
                }

                if (ls_doc_mvt.Equals("TRS") || ls_doc_mvt.Contains("RT") || ls_doc_mvt.Equals("DEF"))  //转移.返退.取消都必须输入备注
                {
                    if (string.IsNullOrEmpty(ls_remark))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请输入备注!");
                        return false;
                    }
                }

                if (ls_doc_mvt.Equals("TRS"))  //转移必须要选择对象厂家
                {
                    if (string.IsNullOrEmpty(ls_move_company) || vend4.Equals(ls_move_company ))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请选择正确的厂家!");
                        return false;
                    }
                }


                if (ls_doc_mvt.Equals("NOR") || ls_doc_type.Equals("LOS"))
                {
                    if (string.IsNullOrEmpty(ls_move_line))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请选择生产线!");
                        return false;
                    }

                    if (string.IsNullOrEmpty(ls_prod_plan_date))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请选择生产日期!");
                        return false;
                    }
                }

                if (ls_doc_type.Equals(type))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "选择类型错误");
                    return false;
                }

                if (ls_doc_mvt.Contains("RTN") && ckCancle.Checked == false)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请先点击出库取消按钮，在选择数据!");
                    return false;
                }

                return true;
            }
            catch(Exception e )
            {
                SecuGlobal.showNG(panelStatus, lblStatus, e.Message );
                return false;
            }
        }



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                if (!bGetValues())
                    return;

                if (!bAnalysis())
                    return;


                DialogResult dr = MessageBox.Show("是否保存修改的内容", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr != DialogResult.OK)
                {
                    SecuGlobal.showOK(panelStatus, lblStatus, "用户取消作业");
                    return;
                }
                
                if (!bInsertData()) //数据写入到TB_SECURITY_DOC
                    return;

                int li_check = iUpdateStockQty(); // 更新库存数量tb_security_stock
                if (li_check == 0)
                {
                    if (ckCancle.Checked)
                        ckCancle.Checked = false;

                    btnApply_Click(sender, e);
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                    Reset(); //数据初始化
                }
                else
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "库存数量更新失败，请联系管理员");
                }

            }
            catch (Exception er)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, er.Message);
            }

        }




        /// <summary>
        /// 更新厂家库存数量
        /// </summary>
        /// <returns></returns>
        private int iUpdateStockQty()
        {
            try
            {
                int li_check;
                if (ls_doc_mvt.Equals("TRS"))  //POP 厂家间转移目前没有，各个厂家都是独立管理
                {
                    //厂家间移动
                    if (ls_move_company.Equals("C660"))
                    {
                        li_check = wf_secu_move(plantInsert, vend4, ls_material_code, -ll_doc_qty); //如果转移到SSDP ，厂家库存数减少
                    }
                    else
                    {
                        //Trans_qty 更新后，Stock 库存缩减
                        li_check = wf_trans_move(plantInsert, vend4, ls_material_code, ll_doc_qty) +
                                    wf_secu_move(plantInsert, vend4, ls_material_code, -ll_doc_qty);
                    }
                }
                else
                {
                    if (ls_doc_type.Equals("GI"))
                    {
                        li_check = wf_secu_move(plantInsert, vend4, ls_material_code, -ll_doc_qty);  //出库，库存数量缩减
                    }
                    else if (ls_doc_type.Equals("GR"))
                    {
                        li_check = wf_secu_move(plantInsert, vend4, ls_material_code, ll_doc_qty); //入库，在库存数量基础上增加对应数
                    }
                    else
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "操作错误，单号取消操作必须选中下方单号后点出库取消操作!");
                        return -1;
                    }
                }

                return li_check;
            }
            catch (Exception)
            {
                return -1;
            }
        }







   


        /// <summary>
        /// 数据写入
        /// 数据完成后，更新库存信息
        /// </summary>
        /// <param name="doc_no"></param>
        /// <returns></returns>
        private bool bInsertData()
        {
            try
            {
                string sql = "select '" + ls_doc_type + "'||to_char(sysdate,'yyyymmdd')||fn_gene_seq('SECU','" + ls_doc_type + "'," +
                "to_char(sysdate,'yyyymmdd'),'N','N','N',4) from dual";
                string ls_doc_no = OracleHelper.ExecuteScalar(sql).ToString();

                string sqlInsert = "";
                string PROD_PLAN_DATE = "",MOVE_COMPANY = "",MOVE_LINE = "",BK_COMPANY= "",BK_DOC_NO="",BK_DOC_SEQ="";

                if (ls_doc_mvt.Equals("RTN"))  //出库取消必须要有 bk_docno  、 ls_bk_company
                {
                    BK_COMPANY = ls_bk_company;
                    BK_DOC_NO = bk_docno;
                    BK_DOC_SEQ = "1";
                }
                else if (ls_doc_mvt.Equals("NOR"))  //正常必须要有 PROD_PLAN_DATE、MOVE_LINE
                {
                    PROD_PLAN_DATE = ls_prod_plan_date;
                    if (ls_move_line.Contains(":"))
                    {
                        string[] split = ls_move_line.Split(new Char[] { ':' });
                        MOVE_LINE = split[0].Trim();
                    }
                    else
                    {
                        MOVE_LINE = ls_move_line;
                    }

                }
                else if (ls_doc_mvt.Equals("TRS"))  //厂家移动必须有MOVE_COMPANY
                {
                    MOVE_COMPANY = ls_move_company;
                }

                sqlInsert = " insert into " + SecuGlobal.tbSecurityDoc + " (COMPANY,DOC_NO,DOC_SEQ,DOC_TYPE,DOC_MVT,MATERIAL_CODE,DOC_QTY," +
                            " PROD_PLAN_DATE,MOVE_COMPANY,MOVE_LINE， " +
                            " DOC_DATE,DOC_TIME,DOC_USER,DOC_IP,BK_COMPANY,BK_DOC_NO,BK_DOC_SEQ,BARCODE_FLAG,PROD_LOTNO,REMARK,PLANT," +
                            " FCT_CODE) " +
                            " VALUES " +
                            " ('" + vend4 + "','" + ls_doc_no + "','1','" + ls_doc_type + "','" + ls_doc_mvt + "','" + ls_material_code + "','" + ll_doc_qty + "', " +
                            " '" + PROD_PLAN_DATE + "','" + MOVE_COMPANY + "','" + MOVE_LINE + "'," +
                            " to_char(sysdate,'yyyymmdd'),to_char(sysdate,'HH24Miss'),'" + PaCSGlobal.LoginUserInfo.Name + "','" + PaCSGlobal.GetClientIp() + "', " +
                            " '" + BK_COMPANY + "','" + BK_DOC_NO + "','" + BK_DOC_SEQ + "','" + barCode + "','" + prodLotNo + "','" + ls_remark + "'," +
                            " '" + plantInsert + "','" + PaCSGlobal.LoginUserInfo.Fct_code + "') ";

                OracleHelper.ExecuteNonQuery(sqlInsert);
                return true;
            }
            catch (Exception)
            {
                return false; 
            }
        }




       /// <summary>
       /// 库存数量增加或者缩减-出库，入库操作
       /// </summary>
       /// <param name="plantinsert"></param>
       /// <param name="vend4"></param>
       /// <param name="ls_material_code"></param>
       /// <param name="ai_doc_qty"></param>
       /// <returns></returns>
        private int wf_secu_move(string plantinsert, string vend4, string ls_material_code, int ai_doc_qty)
        {
            int li_cnt = 0;
            string sql = "select count(*) from " + SecuGlobal.tbSecurityStock + " where company = '" + vend4 + "' " +
                         "and material_code = '" + ls_material_code + "' and plant = '" + plantinsert + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            li_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));

            if (li_cnt == 0)   //POP 此处逻辑判定需要在检讨，个人感觉没有必要增加这个逻辑
            {
                if (ai_doc_qty < 0)
                    return -1;

                string sqlInsert = "  insert  " +
                                    "    into " + SecuGlobal.tbSecurityStock  + " " +
                                    "         (company,material_code,stock_qty, " +
                                    "        update_date,update_time,update_user,update_ip, " +
                                    "        plant,FCT_CODE) " +
                                    "  values ('" + vend4 + "','" + ls_material_code + "','" + ai_doc_qty + "', " +
                                    "          to_char(sysdate,'yyyymmdd'),to_char(sysdate,'HH24Miss'),'" + PaCSGlobal.LoginUserInfo.Name + "','" + PaCSGlobal.GetClientIp() + "', " +
                                    "        '" + plantinsert + "','" + PaCSGlobal.LoginUserInfo.Fct_code  + "') ";
                OracleHelper.ExecuteNonQuery(sqlInsert);
                return 0;
            }

            int li_remain_qty = 0;
            string sql1 = "select nvl(stock_qty,0) + '" + ai_doc_qty + "' from " + SecuGlobal.tbSecurityStock  + " where " +
                         "company = '" + vend4 + "'  and material_code = '" + ls_material_code + "' and plant = '" + plantinsert + "' " +
                         "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            li_remain_qty = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql1));
            if (li_remain_qty < 0)
            {
                XtraMessageBox.Show("库存数负数，请联系管理员");
                return -1;
            }
            else
            {
                if (li_cnt == 1)
                {
                    string sqlUpdate = "      update " + SecuGlobal.tbSecurityStock  + " " +
                                        "         set stock_qty = stock_qty + '" + ai_doc_qty + "', " +
                                        "             update_date = to_char(sysdate,'yyyymmdd'), " +
                                        "             update_time = to_char(sysdate,'HH24Miss'), " +
                                        "             update_user = '" + PaCSGlobal.LoginUserInfo.Name  + "', " +
                                        "             update_ip = '" + PaCSGlobal.GetClientIp() + "' " +
                                        "       where company = '" + vend4  + "' " +
                                        "         and material_code = '" + ls_material_code + "' " +
                                        "         and plant = '" + plantinsert + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                    OracleHelper.ExecuteNonQuery(sqlUpdate);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }

        }




        /// <summary>
        /// 厂家间移库确认，库存数量缩减，TRANS_QTY 数量增加
        /// </summary>
        /// <param name="ls_plant"></param>
        /// <param name="vend4"></param>
        /// <param name="ls_material_code"></param>
        /// <param name="ai_doc_qty"></param>
        /// <returns>0 = OK  </returns>
        private int wf_trans_move(string plantinsert, string vend4, string ls_material_code, int ai_doc_qty)
        {
            int li_cnt = 0;
            string sql = "select count(*) from " + SecuGlobal.tbSecurityStock + " where company = '" + vend4 + "' " +
                         "and material_code = '" + ls_material_code + "' and plant = '" + plantinsert + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            li_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));

            if (li_cnt == 0)
            {
                if (ai_doc_qty < 0)
                    return -1;

                string sqlInsert = "  insert  " +
                                    "    into " + SecuGlobal.tbSecurityStock + " " +
                                    "         (company,material_code,trans_qty, " +
                                    "        update_date,update_time,update_user,update_ip, " +
                                    "        plant,FCT_CODE) " +
                                    "  values ('" + vend4 + "','" + ls_material_code + "','" + ai_doc_qty + "', " +
                                    "          to_char(sysdate,'yyyymmdd'),to_char(sysdate,'HH24Miss'),'" + PaCSGlobal.LoginUserInfo.Name + "','" + PaCSGlobal.GetClientIp() + "', " +
                                    "        '" + plantinsert + "','" + PaCSGlobal.LoginUserInfo.Fct_code  + "') ";
                OracleHelper.ExecuteNonQuery(sqlInsert);
                return 0;
            }

            int li_remain_qty = 0;
            string sql1 = "select nvl(trans_qty,0) + '" + ai_doc_qty + "' from " + SecuGlobal.tbSecurityStock + " where " +
                         "company = '" + vend4 + "'  and material_code = '" + ls_material_code + "' and plant = '" + plantinsert + "' " +
                         "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            li_remain_qty = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql1));
            if (li_remain_qty < 0)
            {
                return -1;
            }
            else
            {
                if (li_cnt == 1)
                {
                    string sqlUpdate = "      update " + SecuGlobal.tbSecurityStock  + " " +
                                        "         set trans_qty = nvl(trans_qty,0) + '" + ai_doc_qty + "', " +
                                        "             update_date = to_char(sysdate,'yyyymmdd'), " +
                                        "             update_time = to_char(sysdate,'HH24Miss'), " +
                                        "             update_user = '" + PaCSGlobal.LoginUserInfo.Name + "', " +
                                        "             update_ip = '" + PaCSGlobal.GetClientIp() + "' " +
                                        "       where company = '" + vend4 + "' " +
                                        "         and material_code = '" + ls_material_code + "' " +
                                        "         and plant = '" + plantinsert + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
                    OracleHelper.ExecuteNonQuery(sqlUpdate);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }




        /// <summary>
        /// 出库取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckCancle_CheckedChanged(object sender, EventArgs e)
        {
            Reset();
            if (ckCancle.Checked)
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "出库取消模式 - 请选择信息，然后保存");
                cbDocMvt.Text = "RTN:" + SecuGlobal.getMoveType("RTN");
                tbDocType.Text = "入库";
                cbDocMvt.Properties.ReadOnly = true;
            }
            else
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正常模式- 请选择信息，然后保存");
                cbDocMvt.Properties.ReadOnly = false;
                cbDocMvt.Text = "";
                tbDocType.Text = "";
                btnSave.Enabled = true;
            }
        }

        /// <summary>
        /// 清空信息
        /// </summary>
        private void Reset()
        {
            tbDesc.Text = ""; tbDocQty.Text = ""; tbDocType.Text = ""; tbProdLotNo.Text = ""; tbRemark.Text = ""; tbStockQty.Text = "";
            cbMaterial.Text = ""; cbMoveCompany.Text = ""; cbMoveLine.Text = ""; deProdPlanDate.Text = "";
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if ((bool)gridView1.GetRowCellValue(i, gridView1.Columns[0]))
                {
                    gridView1.SetRowCellValue(i, gridView1.Columns[0], false);
                }
            }
        }




        private void cbMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMaterial.SelectedIndex == -1)
                return;

            string sql = "";

            string assy = cbMaterial.Text.Trim();
            sql = "select nvl(sum(STOCK_QTY),0) stock_qty from " + SecuGlobal.tbSecurityStock + " where " +
             "material_code = '" + assy + "' AND company = '" + vend4 + "' AND PLANT = '" + plantInsert + "' and " +
             "FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'"; //plantInsert -SSDP OR SESC
            tbStockQty.Text = OracleHelper.ExecuteScalar(sql).ToString();


            sql = "select description from " + SecuGlobal.tb_fpp_itemmaster + " where matnr = '" + assy + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            tbDesc.Text = OracleHelper.ExecuteScalar(sql).ToString();
        }

        private void tbDocQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)  //小数点
            {
                if (tbDocQty.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(tbDocQty.Text, out oldf);
                    b2 = float.TryParse(tbDocQty.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }





    }
}