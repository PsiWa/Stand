using Npgsql;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Stand
{
    internal class PostgreSQLib
    {
        NpgsqlConnection con = null;
        bool isConenected = false;
        private bool InitDB()
        {
            string connStr = "Server=localhost;Username = postgres; Password = 1234; Database = postgres";
            var m_conn = new NpgsqlConnection(connStr);
            m_conn.Open();
            var m_createdb_cmd = new NpgsqlCommand("CREATE DATABASE \"StandResults\" ",m_conn);
            m_createdb_cmd.ExecuteNonQuery();
            m_conn.Close();
            try
            {
                connStr = "Host=localhost;Username=postgres;Password=1234;Database=StandResults";
                m_conn = new NpgsqlConnection(connStr);
                m_conn.Open();
                using (var sr = new StreamReader("DB_StandResults_init.txt"))
                {
                    var m_inittables_cmd = new NpgsqlCommand(sr.ReadToEnd(),m_conn);
                    m_inittables_cmd.ExecuteNonQuery();
                }
                m_conn.Close();
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
                return false;
            }
        }
        public bool NpgsqlConnect()
        {
            if (!isConenected)
            {
                try
                {
                    var connectionString = "Host=localhost;Username=postgres;Password=1234;Database=StandResults";
                    con = new NpgsqlConnection(connectionString);
                    con.Open();
                    isConenected = true;
                    return true;
                }
                catch (Exception)
                {
                    
                    MessageBox.Show("Невозможно подключиться к БД");
                    isConenected = false;
                    DialogResult result = MessageBox.Show("Хотите инициализировать БД?", "Confirmation", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes)
                    {
                        if (InitDB())
                        {
                            var connectionString = "Host=localhost;Username=postgres;Password=1234;Database=StandResults";
                            con = new NpgsqlConnection(connectionString);
                            con.Open();
                            return true;
                        } 
                        else
                            return false;

                    }
                    else if (result == DialogResult.No)
                    {
                        return false;
                    }
                    return false;
                }
            }
            else
            {
                isConenected=true;
                return true;
            }
        }
        public async Task<IEnumerable<string>> TEST()
        {
            var result = new List<string>();
            string sql = "SELECT table_name FROM information_schema.tables WHERE table_schema='public'";
            var cmd = new NpgsqlCommand(sql, con);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                result.Add(rdr["first_name"].ToString());
            }
            return result;
        }
        public async Task<IEnumerable<string>> GetExperimentNames()
        {
            var result = new List<string>();
            string sql = "SELECT experiment_id,experiment_name FROM experiments";
            var cmd = new NpgsqlCommand(sql, con);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                result.Add(rdr["experiment_id"].ToString()+"@"+rdr["experiment_name"].ToString());
            }
            rdr.Close();
            return result;
        }
        public bool SaveMeasurements(ref List<Unit> UnitsList, string expname,ref List<float> timestamp, 
            ref DataTable performanceDT)
        {
            try
            {
                string sql = "INSERT INTO experiments (experiment_name) " +
                        $"VALUES ('{expname}') ON CONFLICT DO NOTHING";
                var cmd = new NpgsqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                sql = $"SELECT experiment_id FROM experiments WHERE experiment_name='{expname}'";
                cmd = new NpgsqlCommand(sql, con);
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                int expid =-1;
                while (rdr.Read())
                {
                    expid = int.Parse(rdr["experiment_id"].ToString());
                }
                rdr.Close();
                if (expid != -1)
                {
                    foreach (Unit unit in UnitsList)
                    {
                        sql = "INSERT INTO units (unit_id,unit_name) " +
                            $"VALUES ({unit.id},'{unit.GetName()}') ON CONFLICT DO NOTHING";
                        cmd = new NpgsqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        foreach (var par in unit.GetParametersList())
                        {
                            sql = "INSERT INTO parameters (par_id,unit_id,par_name) " +
                                $"VALUES ({par.id},{unit.id},'{par.GetName()}') ON CONFLICT DO NOTHING";
                            cmd = new NpgsqlCommand(sql, con);
                            cmd.ExecuteNonQuery();
                            int i = 0;
                            foreach (var m in par.GetAllMeasuredRegs())
                            {
                                if (i >= timestamp.Count())
                                    break;
                                sql = "INSERT INTO measurements (measurement,par_id,experiment_id,time,uom) " +
                                    $"VALUES ({m},{par.id},{expid},{timestamp[i++].ToString().Replace(",",".")},\'{par.GetUoMstring()}\') " +
                                    "ON CONFLICT DO NOTHING";
                                cmd = new NpgsqlCommand(sql, con);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    foreach (DataRow row in performanceDT.Rows)
                    {
                        string[] H = {"-1","-1","-1","-1" };
                        string[] eta = { "-1", "-1", "-1", "-1" };
                        int k = 0;
                        int l = 0;
                        for (int i = 0; i < performanceDT.Columns.Count; i++)
                        {
                            if (performanceDT.Columns[i].ColumnName.Contains("H"))
                                H[k++] = row[i].ToString().Replace(",",".");
                            if (performanceDT.Columns[i].ColumnName.Contains("η"))
                                eta[l++] = row[i].ToString().Replace(",", ".");
                        }
                        if (row[0].ToString() == "1")
                        {
                            sql = "INSERT INTO performance (experiment_id, \"time\", consumption, \"power\", " +
                                "h1, h2, h3, h4, eta1, eta2, eta3, eta4) " +
                                $"VALUES ({expid}, {row[1].ToString().Replace(",", ".")}, " +
                                $"{row[2].ToString().Replace(",", ".")}, {row[3].ToString().Replace(",", ".")}, " +
                                $"{H[0]}, {H[1]}, {H[2]}, {H[3]}, {eta[0]}, {eta[1]}, {eta[2]}, {eta[3]}) " +
                                "ON CONFLICT DO NOTHING";
                            cmd = new NpgsqlCommand(sql, con);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
        
        private void InitDataTable(ref DataTable results, ref DataTable performance, int exp_id)
        {
            DataColumn Timecol = new DataColumn();
            Timecol.ColumnName = $"Time";
            results.Columns.Add(Timecol);

            string query = "SELECT DISTINCT ON (parameters.par_name) parameters.par_name FROM experiments " +
                "JOIN measurements ON experiments.experiment_id = measurements.experiment_id " +
                "JOIN parameters ON parameters.par_id = measurements.par_id " +
                $"WHERE experiments.experiment_id = {exp_id}";
            var cmd = new NpgsqlCommand(query, con);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                DataColumn col = new DataColumn();
                col.ColumnName = (string)rdr["par_name"];
                results.Columns.Add(col);
            }
            rdr.Close();

            DataColumn T = new DataColumn();
            T.ColumnName = $"T";
            performance.Columns.Add(T);

            DataColumn Q = new DataColumn();
            Q.ColumnName = $"Q";
            performance.Columns.Add(Q);

            DataColumn N = new DataColumn();
            N.ColumnName = $"N";
            performance.Columns.Add(N);

            for (int i = 1; i < 5; i++)
            {
                DataColumn H = new DataColumn();
                H.ColumnName = $"H {i}";
                performance.Columns.Add(H);
            }

            for (int i = 1; i < 5; i++)
            {
                DataColumn eta = new DataColumn();
                eta.ColumnName = $"η {i}";
                performance.Columns.Add(eta);
            }
        }
        private void AddToDataTable(ref DataTable results, ref DataTable performance, int exp_id)
        {
            string query = "SELECT DISTINCT ON (measurements.time) measurements.time FROM experiments " +
                "JOIN measurements ON experiments.experiment_id = measurements.experiment_id "+
                $"WHERE experiments.experiment_id = {exp_id}";
            var cmd = new NpgsqlCommand(query, con);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                DataRow row = results.NewRow();
                row["Time"] = rdr["time"].ToString();
                results.Rows.Add(row);
            }
            rdr.Close();

            foreach (DataRow row in results.Rows)
            {
                query = "SELECT * FROM experiments " +
                    "JOIN measurements ON experiments.experiment_id = measurements.experiment_id " +
                    "JOIN parameters ON parameters.par_id = measurements.par_id " +
                    "JOIN units ON units.unit_id = parameters.unit_id " +
                    $"WHERE experiments.experiment_id = {exp_id} " +
                    $"AND measurements.time = '{row["Time"].ToString().Replace(",", ".")}' " +
                    $"ORDER BY measurements.time";
                cmd = new NpgsqlCommand(query, con);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    row[rdr["par_name"].ToString()] = rdr["measurement"].ToString();
                }
                rdr.Close();
            }

            query = "SELECT * FROM performance " +
                    $"WHERE performance.experiment_id = {exp_id} " +
                    $"ORDER BY performance.time";
            cmd = new NpgsqlCommand(query, con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                DataRow row = performance.NewRow();
                row["T"] = rdr["time"].ToString();
                row["Q"] = rdr["consumption"].ToString();
                row["N"] = rdr["power"].ToString();
                for (int i = 1; i < 5; i++)
                {
                    row[$"H {i}"] = rdr[$"h{i}"].ToString();
                    row[$"η {i}"] = rdr[$"eta{i}"].ToString();
                }
                performance.Rows.Add(row);
            }
            rdr.Close();

        }
        public bool GetExperimentResults(int exp_id, ref DataTable results, ref DataTable performance)
        {
            try
            {
                InitDataTable(ref results, ref performance, exp_id);
                AddToDataTable(ref results, ref performance, exp_id);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public bool DeleteExperiment(int exp_id)
        {
            try
            {
                string sql = $"DELETE FROM experiments WHERE experiment_id = {exp_id}";
                var cmd = new NpgsqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
        public bool ChangeExperimentName(int exp_id, string newName)
        {
            try
            {
                string sql = $"UPDATE experiments SET experiment_name = '{newName}' WHERE experiment_id = {exp_id}";
                var cmd = new NpgsqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
    }
}
