using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GCAL_Strings_Editor
{
    public partial class Form1 : Form
    {
        public List<Stringrec> items = new List<Stringrec>();
        public ListViewItem currItem = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currItem != null)
            {
                saveItem();
            }

            if (listView1.SelectedItems.Count > 0)
            {
                currItem = listView1.SelectedItems[0];
                loadItem();
                richTextBox1.Focus();
            }
        }

        private void saveItem()
        {
            if (currItem != null)
            {
                Stringrec sr = currItem.Tag as Stringrec;
                sr.Text = richTextBox1.Text;
                sr.OriginalText = richTextBox2.Text;
                sr.Description = richTextBox3.Text;
                sr.Key = textBox2.Text;

                if (sr.OriginalText.Length == 0)
                    sr.OriginalText = sr.Text;

                currItem.SubItems[1].Text = sr.Text;
                currItem.SubItems[2].Text = sr.Key;
                currItem.SubItems[3].Text = sr.Description;
                currItem.SubItems[4].Text = sr.OriginalText;
                updateHighlight(currItem);
            }
        }

        private void loadItem()
        {
            if (currItem != null)
            {
                Stringrec sr = currItem.Tag as Stringrec;
                richTextBox1.Text = sr.Text;
                richTextBox2.Text = sr.OriginalText;
                richTextBox3.Text = sr.Description;
                textBox2.Text = sr.Key;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                readFile(openFileDialog1.FileName);
                updateListView();
            }
        }

        private void setItem(Stringrec sr)
        {
            while (items.Count <= sr.Id)
                items.Add(new Stringrec());
            items[sr.Id] = sr;
        }

        private void readFile(string fileName)
        {
            label3.Text = fileName;
            string bakPath = fileName + ".bak";
            string bakText = File.ReadAllText(fileName);
            File.WriteAllText(bakPath, bakText);
            using (StreamReader sr = new StreamReader(fileName))
            {
                string s = sr.ReadLine();
                while (s != null)
                {
                    string [] parts = s.Split('\t');
                    if (parts.Length > 1)
                    {
                        if (parts[0] == "lang")
                            textBox1.Text = Stringrec.getUnicodeFromHtml(parts[1]);
                        else if (parts[0] == "langid")
                            textBox3.Text = parts[1];
                        else
                            setItem(new Stringrec(parts));
                    }
                    s = sr.ReadLine();
                }
            }
        }

        private void saveFile()
        {
            saveItem();
            using (StreamWriter sw = new StreamWriter(label3.Text))
            {
                sw.WriteLine("lang\t{0}", Stringrec.getHtmlFromUnicode(textBox1.Text));
                sw.WriteLine("langid\t{0}", textBox3.Text);
                foreach (Stringrec sr in items)
                {
                    if (sr.Id >= 0)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", sr.Id, sr.TextHtml, sr.KeyHtml, sr.DescriptionHtml, sr.OriginalTextHtml);
                    }
                }
            }
        }

        private void updateListView()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (Stringrec sr in items)
            {
                if (sr.Id >= 0)
                {
                    ListViewItem lvi = new ListViewItem(sr.Id.ToString());
                    sr.lvi = lvi;
                    lvi.Tag = sr;
                    lvi.SubItems.Add(sr.Text);
                    lvi.SubItems.Add(sr.Key);
                    lvi.SubItems.Add(sr.Description);
                    lvi.SubItems.Add(sr.OriginalText);
                    updateHighlight(lvi);
                    listView1.Items.Add(lvi);
                }
            }
            listView1.EndUpdate();
        }

        private void updateHighlight(ListViewItem lvi)
        {
            if (lvi.SubItems.Count > 4)
            {
                if ((lvi.SubItems[4].Text != lvi.SubItems[1].Text) && checkBox1.Checked)
                {
                    lvi.ForeColor = Color.Blue;
                }
                else
                {
                    lvi.ForeColor = Color.Black;
                }
            }
            else
            {
                lvi.ForeColor = Color.Green;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // import english original from file
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                {
                    string s = sr.ReadLine();
                    while (s != null)
                    {
                        string[] parts = s.Split('\t');
                        if (parts.Length > 1)
                        {
                            if (parts[0] == "lang") {}
                            else if (parts[0] == "langid") { }
                            else
                            {
                                Stringrec nx = new Stringrec(parts);
                                Stringrec ex = findItem(items, nx.Id);
                                if (ex != null)
                                {
                                    ex.OriginalText = nx.Text;
                                    if (ex.lvi != null)
                                        ex.lvi.SubItems[4].Text = nx.Text;
                                }
                            }
                        }
                        s = sr.ReadLine();
                    }
                }
            }
        }

        public Stringrec findItem(List<Stringrec> list, int id)
        {
            foreach (Stringrec sr in list)
            {
                if (sr.Id == id)
                    return sr;
            }
            return null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = richTextBox1.Text;
            if (s.Length > 0)
            {
                string d = s.Substring(0, 1) + s.Substring(1);
                richTextBox1.Text = d;
            }
        }

        private void button5_AddNewString_Click(object sender, EventArgs e)
        {
            int max = 0;
            foreach (Stringrec sr in items)
            {
                max = Math.Max(max, sr.Id);
            }

            Stringrec nr = new Stringrec();
            nr.Id = max + 1;
            items.Add(nr);

            ListViewItem lvi = new ListViewItem();
            lvi.Text = nr.Id.ToString();
            lvi.Tag = nr;
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            nr.lvi = lvi;
            updateHighlight(lvi);
            listView1.Items.Add(lvi);
            listView1.EnsureVisible(lvi.Index);
            lvi.Selected = true;
            loadItem();
            richTextBox1.Focus();
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView2.SelectedItems[0];
                Stringrec sr = lvi.Tag as Stringrec;
                if (sr != null)
                {
                    listView1.EnsureVisible(sr.lvi.Index);
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string part = textBox4.Text;
            listView2.BeginUpdate();
            listView2.Items.Clear();
            foreach (Stringrec sr in items)
            {
                if (sr.Text == null)
                    continue;
                if (sr.Text.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ListViewItem lvi = new ListViewItem(sr.Id.ToString());
                    lvi.SubItems.Add(sr.Text);
                    lvi.Tag = sr;
                    listView2.Items.Add(lvi);
                }
            }
            listView2.EndUpdate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // import english original from file
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveItem();
                currItem = null;
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                {
                    string s = sr.ReadLine();
                    while (s != null)
                    {
                        string[] parts = s.Split('\t');
                        if (parts.Length > 1)
                        {
                            if (parts[0] == "lang") { }
                            else if (parts[0] == "langid") { }
                            else
                            {
                                Stringrec nx = new Stringrec(parts);
                                Stringrec ex = findItem(items, nx.Id);
                                if (ex == null)
                                {
                                    setItem(nx);
                                }
                            }
                        }
                        s = sr.ReadLine();
                    }
                }
                updateListView();
            }
        }
    }

    public class Stringrec
    {
        public int Id = -1;
        public string Text;
        public string OriginalText;
        public string Description;
        public string Key;
        public ListViewItem lvi = null;


        public string TextHtml
        {
            get { return getHtmlFromUnicode(Text); }
            set { Text = getUnicodeFromHtml(value); }
        }

        public string OriginalTextHtml
        {
            get { return getHtmlFromUnicode(OriginalText); }
            set { OriginalText = getUnicodeFromHtml(value); }
        }

        public string DescriptionHtml
        {
            get { return getHtmlFromUnicode(Description); }
            set { Description = getUnicodeFromHtml(value); }
        }

        public string KeyHtml
        {
            get { return getHtmlFromUnicode(Key); }
            set { Key = getUnicodeFromHtml(value); }
        }

        public Stringrec()
        {
        }

        public Stringrec(string[] rec)
        {
            if (rec.Length > 0)
                int.TryParse(rec[0], out Id);
            if (rec.Length > 1)
                TextHtml = rec[1];
            if (rec.Length > 2)
                KeyHtml = rec[2];
            if (rec.Length > 3)
                DescriptionHtml = rec[3];
            if (rec.Length > 4)
                OriginalTextHtml = rec[4];
        }

        public static string getHtmlFromUnicode(string s)
        {
            if (s == null)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c == '&')
                {
                    sb.Append("&amp;");
                }
                else if (c == '<')
                {
                    sb.Append("&lt;");
                }
                else if (c == '>')
                {
                    sb.Append("&gt;");
                }
                else if (Convert.ToInt32(c) >= 128)
                {
                    sb.AppendFormat("&#{0};", Convert.ToInt32(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string getUnicodeFromHtml(string s)
        {
            if (s == null)
                return "";
            StringBuilder sb = new StringBuilder();
            int mode = 0;
            for(int i = 0; i < s.Length; i++)
            {
                char u = s[i];
                if (mode == 0)
                {
                    if (u == '&')
                    {
                        int end = s.IndexOf(';', i);
                        if (end >= 0)
                        {
                            string sub = s.Substring(i, end - i + 1);
                            i = end;
                            if (sub == "&amp;")
                            {
                                sb.Append('&');
                            }
                            else if (sub == "&gt;")
                            {
                                sb.Append(">");
                            }
                            else if (sub == "&lt;")
                            {
                                sb.Append("<");
                            }
                            else if (sub.StartsWith("&#"))
                            {
                                int io;
                                int.TryParse(sub.Substring(2, sub.Length - 3), out io);
                                sb.Append(Convert.ToChar(io));
                            }
                        }
                        else
                        {
                            sb.Append(u);
                        }
                    }
                    else
                    {
                        sb.Append(u);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
