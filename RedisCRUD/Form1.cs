using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RedisCRUD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        void Normal()
        {
            btnDelete.Enabled = true;
            btnAdd.Enabled = true;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            txtID.Enabled = false;
            txtName.Enabled = false;
            txtAddress.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (RedisClient client = new RedisClient("127.0.0.1", 6379, ""))
            {
                IRedisTypedClient<Student> phone = client.As<Student>();
                studentBindingSource.DataSource = phone.GetAll();
                Normal();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            studentBindingSource.Add(new Student());
            studentBindingSource.MoveLast();
            txtID.Focus();
            txtID.Enabled = true;
            txtName.Enabled = true;
            txtAddress.Enabled = true;
            btnDelete.Enabled = false;
            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txtID.Focus();
            txtID.Enabled = false;
            txtName.Enabled = true;
            txtAddress.Enabled = true;
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure want to delete this record ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var p = studentBindingSource.Current as Student;//Get current object
                if (p != null)
                {
                    //Connect to your redis cache
                    using (RedisClient client = new RedisClient("127.0.0.1", 6379, ""))
                    {
                        IRedisTypedClient<Student> student = client.As<Student>();
                        student.DeleteById(p.ID);
                        client.Remove(txtID.Text);
                        studentBindingSource.RemoveCurrent();
                    }
                }
            }
            Normal();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var redisClient = new RedisClient("127.0.0.1", 6379, ""))
            {
                var Sv = new Student();
                Sv.ID = txtID.Text;
                Sv.Name = txtName.Text;
                Sv.Address = txtAddress.Text;
                redisClient.Set(Sv.ID, Sv);
            }
            MessageBox.Show(this, "Your data has been successfully saved.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Normal();
        }

        public class Student
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Normal();
        }
    }
}
