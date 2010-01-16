using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSint
{
    public class Base
    {
        public String name;
        private long lg;
        private double db;
        private String str;
        private String use; // Show using part of class: Long - lg, Double - db, String - str, Empty - nothing, Error - error;

        public Base()
        {
            Clear();
            name = "Noname";
        }
        public Base(long rhs)
        {
            Clear();
            lg = rhs;
            use = "Long";
            name = "Noname";
        }
        public Base(double rhs)
        {
            Clear();
            db = rhs;
            use = "Double";
            name = "Noname";
        }
        public Base(String Rname)
        {
            Clear();
            name = Rname;
            use = "String";
        }
        public Base(String Rname,long rhs)
        {
            Clear();
            lg = rhs;
            name = Rname;
            use = "Long";           
        }
        public Base(String Rname,double rhs)
        {
            Clear();
            db = rhs;
            name = Rname;
            use = "Double";
        }
        public Base(String Rname,String rhs)
        {
            Clear();
            str = rhs;
            name = Rname;
            use = "String";
        }
        public void Clear() // Change variable value to null. Name doesn't change
        {
            use = "Empty";
            lg = 0;
            db = 0;
            str = "";
        }
        public static Base operator +(Base a, Base b)
        {
            if (a.use == b.use)
            {
                a.lg += b.lg;
                a.db += b.db;
                a.str += b.str;
            }
            return a;
        }
        public static Base operator -(Base a, Base b)
        {
            if (a.use == b.use)
            {
                a.lg -= b.lg;
                a.db -= b.db;
                /*while (a.str.IndexOf(b.str) != -1)
                {
                    a.str.Remove(a.str.IndexOf(b.str), a.str.IndexOf(b.str) + b.str.Length);
                }*/
            }
            return a;
        }
        public static Base operator *(Base a, Base b)
        {
            if (a.use == b.use)
            {
                if(a.use == "Long")
                    a.lg *= b.lg;
                if(a.use == "Double")
                    a.db *= b.db;
                //a.str *= b.str;
            }
            return a;
        }
        public static Base operator /(Base a, Base b)
        {
            if (a.use == b.use)
            {
                if (a.use == "Long")
                    a.lg *= b.lg;
                if (a.use == "Double")
                    a.db *= b.db;
                //a.str /= b.str;
            }
            return a;
        }
    }
    public class Function
    {
        private List<Base> vrb;
        private String input;
        private String output;
        private String funcName;
        TextBox code;
        private int posi;
        private int posj;

        public void GetCode(TextBox codeBox)
        {
            code = codeBox;
        }
        public void GetCode(String fileName)
        {
        }
        public Base FunctionUse(String name,List<Base> inVrb)
        {
            return inVrb[0];
        }
        private string Read()
        {
            return "11";
        }
    }
    public class Methods
    {
    }
}
