using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Stand
{
    public class Scheme
    {
        static public float g_constant = (float)9.8155; 
        static private int MaxN = 0;
        public int SchemeN;
        public string name;

        public int PressureUID;
        public int PressureP1ID;
        public int PressureP2ID;
        public int PressureP3ID;
        public int PressureP4ID;
        public int PressureP5ID;

        public int FCUID;
        public int FCPID;
        public int FCPPID;

        public int ValveUID;
        public int ValvePID;

        public int FlowUID;
        public int FlowPID;

        public float Density;
        public float[] dL = new float[4] { 0, 0, 0, 0 };
        public float D;

        public Scheme()
        {
            SchemeN = MaxN++;
            name = $"Конфигурация {SchemeN}";
            PressureUID = -1;
            PressureP1ID = -1;
            PressureP2ID = -1;
            PressureP3ID = -1;
            PressureP4ID = -1;
            PressureP5ID = -1;
            FCUID = -1;
            FCPID = -1;
            FCPPID = -1;
            ValveUID = -1;
            ValvePID = -1;
            FlowUID = -1;
            FlowPID = -1;
            Density = 0;
            D = 0;
        }
        public Scheme(XElement el)
        {
            LoadXML(el);
            SchemeN = MaxN++;
        }
        public void LoadXML(XElement el)
        {
            name = el.Attribute("Name").Value;
            PressureUID = Convert.ToInt32(el.Attribute("PressureU").Value);
            PressureP1ID = Convert.ToInt32(el.Attribute("PressureP1").Value);
            PressureP2ID = Convert.ToInt32(el.Attribute("PressureP2").Value);
            PressureP3ID = Convert.ToInt32(el.Attribute("PressureP3").Value);
            PressureP4ID = Convert.ToInt32(el.Attribute("PressureP4").Value);
            PressureP5ID = Convert.ToInt32(el.Attribute("PressureP5").Value);

            FlowUID = Convert.ToInt32(el.Attribute("FlowU").Value);
            FlowPID = Convert.ToInt32(el.Attribute("FlowP").Value);

            FCUID = Convert.ToInt32(el.Attribute("FCU").Value);
            FCPID = Convert.ToInt32(el.Attribute("FCP").Value);
            FCPPID = Convert.ToInt32(el.Attribute("FCPP").Value);

            ValveUID = Convert.ToInt32(el.Attribute("ValveU").Value);
            ValvePID = Convert.ToInt32(el.Attribute("ValveP").Value);

            string stringVal = el.Attribute("L1").Value;
            if (stringVal.Contains("."))
                stringVal = stringVal.Replace(".", ",");
            dL[0] = Convert.ToSingle(stringVal);

            stringVal = el.Attribute("L2").Value;
            if (stringVal.Contains("."))
                stringVal = stringVal.Replace(".", ",");
            dL[1] = Convert.ToSingle(stringVal);

            stringVal = el.Attribute("L3").Value;
            if (stringVal.Contains("."))
                stringVal = stringVal.Replace(".", ",");
            dL[2] = Convert.ToSingle(stringVal);

            stringVal = el.Attribute("L4").Value;
            if (stringVal.Contains("."))
                stringVal = stringVal.Replace(".", ",");
            dL[3] = Convert.ToSingle(stringVal);

            stringVal = el.Attribute("Density").Value;
            if (stringVal.Contains("."))
                stringVal = stringVal.Replace(".", ",");
            Density = Convert.ToSingle(stringVal);

            stringVal = el.Attribute("D").Value;
            if (stringVal.Contains("."))
                stringVal = stringVal.Replace(".", ",");
            D = Convert.ToSingle(stringVal);
        }
        public void SaveXML(ref List<Unit> UL,TextBox SchemeNameTB, ComboBox ConfigCB, ComboBox PressUIDCB
            , ComboBox PressP1CB, ComboBox PressP2CB, ComboBox PressP3CB, ComboBox PressP4CB, ComboBox PressP5CB
            , ComboBox FlowUCB, ComboBox FlowPCB, ComboBox FCUCB, ComboBox FCPCB, ComboBox FCPPCB, ComboBox ValveUCB
            , ComboBox ValvePCB, TextBox DensityTB, TextBox L1TB, TextBox L2TB, TextBox L3TB, TextBox L4TB, TextBox DTB)
        {
            Form1.xSettings.Elements("scheme").Where(s => s.Attribute("Name").Value == this.name).Remove();
            XElement xScheme = new XElement("scheme");

            name = SchemeNameTB.Text;
            XAttribute xName = new XAttribute("Name", name);

            var un = UL.Find(u => u.GetName() == PressUIDCB.Text);
            PressureUID = un.id;
            var PL = un.GetParametersList();
            if (PressP1CB.Text != "---")
                PressureP1ID = PL.Find(p => p.GetName() == PressP1CB.Text).id;
            else 
                PressureP1ID = -1;
            if (PressP2CB.Text != "---")
                PressureP2ID = PL.Find(p => p.GetName() == PressP2CB.Text).id;
            else
                PressureP2ID = -1;
            if (PressP3CB.Text != "---")
                PressureP3ID = PL.Find(p => p.GetName() == PressP3CB.Text).id;
            else
                PressureP3ID = -1;
            if (PressP4CB.Text != "---")
                PressureP4ID = PL.Find(p => p.GetName() == PressP4CB.Text).id;
            else
                PressureP4ID = -1;
            if (PressP5CB.Text != "---")
                PressureP5ID = PL.Find(p => p.GetName() == PressP5CB.Text).id;
            else
                PressureP5ID = -1;
            XAttribute xPressureU = new XAttribute("PressureU", PressureUID);
            XAttribute xPressureP1 = new XAttribute("PressureP1", PressureP1ID);
            XAttribute xPressureP2 = new XAttribute("PressureP2", PressureP2ID);
            XAttribute xPressureP3 = new XAttribute("PressureP3", PressureP3ID);
            XAttribute xPressureP4 = new XAttribute("PressureP4", PressureP4ID);
            XAttribute xPressureP5 = new XAttribute("PressureP5", PressureP5ID);

            FlowUID = UL.Find(u => u.GetName() == FlowUCB.Text).id;
            FlowPID = UL.Find(u => u.GetName() == FlowUCB.Text).GetParametersList()
                .Find(p => p.GetName() == FlowPCB.Text).id;
            XAttribute xFlowU = new XAttribute("FlowU", FlowUID);
            XAttribute xFlowP = new XAttribute("FlowP", FlowPID);

            FCUID = UL.Find(u => u.GetName() == FCUCB.Text).id;
            FCPID = UL.Find(u => u.GetName() == FCUCB.Text).GetParametersList()
                .Find(p => p.GetName() == FCPCB.Text).id;
            FCPPID = UL.Find(u => u.GetName() == FCUCB.Text).GetParametersList()
                .Find(p => p.GetName() == FCPPCB.Text).id;
            XAttribute xFCU = new XAttribute("FCU", FCUID);
            XAttribute xFCP = new XAttribute("FCP", FCPID);
            XAttribute xFCPP = new XAttribute("FCPP", FCPPID);

            if (ValveUCB.Text != "---" || ValvePCB.Text != "---")
            {
                ValveUID = UL.Find(u => u.GetName() == ValveUCB.Text).id;
                ValvePID = UL.Find(u => u.GetName() == ValveUCB.Text).GetParametersList()
                    .Find(p => p.GetName() == ValvePCB.Text).id;
            }
            else
            {
                ValveUID = -1;
                ValvePID = -1;
            }
            XAttribute xValveU = new XAttribute("ValveU", ValveUID);
            XAttribute xValveP = new XAttribute("ValveP", ValvePID);

            dL[0] = Convert.ToSingle(L1TB.Text);
            dL[1] = Convert.ToSingle(L2TB.Text);
            dL[2] = Convert.ToSingle(L3TB.Text);
            dL[3] = Convert.ToSingle(L4TB.Text);
            XAttribute xL1 = new XAttribute("L1", dL[0]);
            XAttribute xL2 = new XAttribute("L2", dL[1]);
            XAttribute xL3 = new XAttribute("L3", dL[2]);
            XAttribute xL4 = new XAttribute("L4", dL[3]);

            Density = Convert.ToSingle(DensityTB.Text);
            XAttribute xDensity = new XAttribute("Density", (float)Density);

            D = Convert.ToSingle(DTB.Text);
            XAttribute xD = new XAttribute("D", D);

            xScheme.Add(xName, xPressureU, xPressureP1, xPressureP2, xPressureP3, xPressureP4, xPressureP5,
                xFlowU, xFlowP, xFCU, xFCP, xFCPP, xValveU, xValveP, xL1, xL2, xL3, xL4, xDensity, xD);
            Form1.xSettings.Add(xScheme);
            Form1.xSettings.Save("Defaults.xml");
        }
    }
}
