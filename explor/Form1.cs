using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.InteropServices;

namespace explor
{
    public partial class Form1 : Form
    {
        int index = 0;
        ArrayList list = new ArrayList();
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadAllDevicesToTreeView();
            toolStripButton2.Enabled = false;
            toolStripButton1.Enabled = false;
        }
        /// <summary>  
        /// 加载所有磁盘  
        /// </summary>  
        public void LoadAllDevicesToTreeView()
        {
            DriveInfo[] driver = DriveInfo.GetDrives();
            foreach (DriveInfo drv in driver)
            {
                TreeNode tnode = new TreeNode(drv.Name, 0, 0);
                tnode.Name = drv.Name;
                if (drv.IsReady)
                {
                    tnode.ToolTipText = drv.Name + "磁盘大小:" + drv.TotalSize;
                    LoadFolderInFoldersToTV(drv.Name, tnode);//node C:/
                    this.treeView1.Nodes.Add(tnode);
                }
            }
        }
        /// <summary>
        /// 显示目录树
        /// </summary>
        /// <param name="folder">目录</param>
        /// <param name="node"></param>
        public void LoadFolderInFoldersToTV(string folder, TreeNode node)
        {
            string[] folders = Directory.GetDirectories(folder);
            foreach (string fod in folders)
            {
                if ((File.GetAttributes(fod) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                TreeNode tnode = new TreeNode(Path.GetFileName(fod), 0,0);
                tnode.Name = fod;
                tnode.ToolTipText = "文件夹" + fod;
                node.Nodes.Add(tnode);
            }
        }  
        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode tnode in e.Node.Nodes)
            {
                tnode.Nodes.Clear();//每次操作都清空上一步的节点操作，
                this.LoadNextFolderInFoldersToTV(tnode.Name, tnode);
            }  
        }
        /// <summary>  
        /// 加号展开后的显示，为了保留住C:\ D：\ ... 
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        public void LoadNextFolderInFoldersToTV(string folder, TreeNode node)
        {
            string[] folders = Directory.GetDirectories(folder);
            foreach (string fod in folders)
            {
                if ((File.GetAttributes(fod) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                TreeNode tnode = new TreeNode(Path.GetFileName(fod), 0, 0);
                tnode.Name = fod;
                tnode.ToolTipText = "文件夹" + fod;
                node.Nodes.Add(tnode);
            }
        }  
 
        /// <summary>
        /// treeview选定后，list view中的项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            toolStripTextBox1.Text = e.Node.Name;
            LoadAllFileInFolder(e.Node.Name);  
        }
        /// <summary>  
        /// 取得文件夹中的文件，加到lstviewDetail中 
        /// 需要加载记忆
        /// </summary>  
        /// <param name="folderName"></param>  
        private void LoadAllFileInFolder(string folder)
        {
            

            if (folder.Contains("System Volume Information"))
            {
                return;
            }
            this.lstviewDetail.Items.Clear();
            
            //问题：显示的都是文件的名称，不是地址
            string[] fodtxt = Directory.GetDirectories(@folder);
            string []filestr=Directory.GetFiles(@folder);
            ListViewItem[] lvl = new ListViewItem[fodtxt.Length+filestr.Length];
            //访问文件夹
            int i=0;
            this.lstviewDetail.LargeImageList = imageList1;
            for (i =0; i < fodtxt.Length; i++)
            {
                    lvl[i] = new ListViewItem();
                    lvl[i].Text = Path.GetFileName(fodtxt[i]);//设置item的text为文件名
                    lvl[i].Tag = fodtxt[i];//设置item的tag属性为文件路径
                    this.lstviewDetail.Items.Add(lvl[i]);
                    lvl[i].ImageIndex = 0;
            }
            
            //访问文件
            for (int j = 0; j < filestr.Length;j++ )
            {
                lvl[i+j] = new ListViewItem();
                lvl[i + j].Text = Path.GetFileName(filestr[j]);//设置item的text为文件名
                lvl[i + j].Tag = filestr[j];//设置item的tag属性为文件路径
                this.lstviewDetail.Items.Add(lvl[i + j]);
                //lvl[i + j].ImageIndex = 1;
            }
            this.lstviewDetail.LargeImageList = imageList2;
        }

        [DllImport("shell32.DLL", EntryPoint = "ExtractAssociatedIcon")]
        private static extern int ExtractAssociatedIconA(int hInst, string lpIconPath, ref int lpiIcon); //声明函数
        System.IntPtr thisHandle;
        public System.Drawing.Icon SetIcon(string path)
        {
            int RefInt = 0;
            thisHandle = new IntPtr(ExtractAssociatedIconA(0, path, ref RefInt));
            return System.Drawing.Icon.FromHandle(thisHandle);
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
           
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
           
        }
        

         private void treeView1_DoubleClick(object sender, EventArgs e)
         {
             
         }

         private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
         {

         }

         private void lstviewDetail_ItemActivate(object sender, EventArgs e)
         {

         }
        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         private void lstviewDetail_MouseDoubleClick(object sender, MouseEventArgs e)
         {
             //打开一个新的页面  记忆
             toolStripButton1.Enabled = true;
             if (index == 0)
             {
                 if (index != list.Count)
                 {
                     list.RemoveRange(index, list.Count - 1);
                     toolStripButton2.Enabled = false;
                 }
                 if (list.Count < 5)
                 {
                     list.Add(this.toolStripTextBox1.Text);
                     index++;
                 }
                 else
                 {
                     list.RemoveAt(0);
                     list.Add(this.toolStripTextBox1.Text);
                 }

             }
             ListView item=(ListView)sender;
             this.toolStripTextBox1.Text = item.SelectedItems[0].Tag.ToString();
             if (index != list.Count)
             {
                 //for (int i = 0; i < list.Count - index; i++)
                 //{
                 //    list.RemoveAt(index );
                 //}
                 list.RemoveRange(index,list.Count-1);
                 toolStripButton2.Enabled = false;
             }
             if (list.Count < 5)
             {
                 list.Add(this.toolStripTextBox1.Text);
                 index++;
             }
             else
             {
                 list.RemoveAt(0);
                 list.Add(this.toolStripTextBox1.Text);
             }
             //判断是不是文件夹
             if (System.IO.Directory.Exists(toolStripTextBox1.Text))
             {
                 LoadAllFileInFolder(item.SelectedItems[0].Tag.ToString());
             }
             else if (System.IO.File.Exists(toolStripTextBox1.Text))
             {
                 Process.Start(toolStripTextBox1.Text);
             }
             
         }
        //新建文件夹
         private void toolStripMenuItem1_Click(object sender, EventArgs e)
         {
            //路径拼接..有问题：假如当前的list view中没有item 怎么获取地址
             string exampleFile = this.toolStripTextBox1.Text;
             string newFile=exampleFile.Substring(0,exampleFile.LastIndexOf(@"\"))+@"\"+"新建文件夹";
             //新建文件夹有两种情况：1 2 ；已有新建文件夹1，新建文件夹2.；没有新建文件夹
             int i = 0;
             string str = newFile;
             while (Directory.Exists(str))
             {
                 str = newFile + "(" + i + ")";
                 i++;
             }
             Directory.CreateDirectory(str);
             //新建文件夹后刷新
             LoadAllFileInFolder(this.toolStripTextBox1.Text);
         }
        //listview mousedown事件
         private void lstviewDetail_MouseDown(object sender, MouseEventArgs e)
         {
             //判断是不是右键
             if (e.Button == MouseButtons.Right)
             {
                 //判断是不是选中了item
                 if (this.lstviewDetail.SelectedItems.Count == 0)
                 {
                     this.lstviewDetail.ContextMenuStrip = this.contextMenuStrip1;
                 }
                 else
                 {
                     this.lstviewDetail.ContextMenuStrip = this.contextMenuStrip2;
                 }
             }
         }
        /// <summary>
        /// 后退 每一次单击和双击，都把tag属性记忆下来，发送到数组内
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         private void toolStripButton1_Click(object sender, EventArgs e)
         {
             if (index > 1)
             {
                 LoadAllFileInFolder(list[index - 2].ToString());
                 toolStripTextBox1.Text = list[index - 2].ToString();
                 index--;
                 if (index == 1)
                 {
                     toolStripButton1.Enabled = false;
                 }
                 toolStripButton2.Enabled = true;
             }            
         }
         /// <summary>
         /// 前进按钮
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void toolStripButton2_Click(object sender, EventArgs e)
         {
             if (index < 5)
             {
                 toolStripButton1.Enabled = true;
                 LoadAllFileInFolder(list[index].ToString());
                 toolStripTextBox1.Text = list[index].ToString();
                 index++;
                 if (index == list.Count)
                 {
                     toolStripButton2.Enabled = false;
                 }
             }
         }
        //fuzhi
         string sourcePath;
         string name;
         bool flag = true; //true为剪切，false复制

         private void 复制ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             sourcePath = this.lstviewDetail.SelectedItems[0].Tag.ToString();
             name = this.lstviewDetail.SelectedItems[0].Text;
             flag = false;

         }

         private void 粘贴ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             string targetPath = toolStripTextBox1.Text + @"\" + name;
             if (flag)
             {
                 if (System.IO.Directory.Exists(sourcePath))
                 {
                     System.IO.Directory.Move(sourcePath, targetPath);
                 }
                 else if (System.IO.File.Exists(sourcePath))
                 {
                     System.IO.File.Move(sourcePath, targetPath);
                 }
             }
             else
             {
                 if (System.IO.Directory.Exists(sourcePath))
                 {
                     if (!System.IO.Directory.Exists(targetPath))
                     {
                         System.IO.Directory.CreateDirectory(targetPath);
                     }
                     if (System.IO.Directory.Exists(sourcePath))
                     {
                         string[] files = System.IO.Directory.GetFiles(sourcePath);

                         // Copy the files and overwrite destination files if they already exist.
                         string fileName;
                         string destFile;
                         foreach (string s in files)
                         {
                             // Use static Path methods to extract only the file name from the path.
                             fileName = System.IO.Path.GetFileName(s);
                             destFile = System.IO.Path.Combine(targetPath, fileName);
                             System.IO.File.Copy(s, destFile, true);
                         }
                     }
                     else
                     {
                         Console.WriteLine("文件不存在!");
                     }

                 }
                 else if (System.IO.File.Exists(sourcePath))
                 {
                     System.IO.File.Copy(sourcePath, targetPath);
                 }
             }
            //更新
             LoadAllFileInFolder(toolStripTextBox1.Text);

         }
        
         private void 剪切ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             sourcePath = this.lstviewDetail.SelectedItems[0].Tag.ToString();
             name = this.lstviewDetail.SelectedItems[0].Text;
             flag = true;

         }

         private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             if (lstviewDetail.SelectedItems.Count == 0)
                 return;
             DialogResult result = MessageBox.Show("确定要删除吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
             if (result == DialogResult.No)
                 return;
             try
             {
                 foreach (ListViewItem item in lstviewDetail.SelectedItems)
                 {
                     string path = item.Name;
                     if (File.Exists(path))  //文件
                         File.Delete(path);
                     else if (Directory.Exists(path))    //目录
                         Directory.Delete(path, true);
                     lstviewDetail.Items.Remove(item);
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
             }
             //更新
             //LoadAllFileInFolder(toolStripTextBox1.Text);
         }
    }
}
