using System.Security.Cryptography;

namespace BNRNew_API.config
{
    public class AppConstant
    {
        //##########################################################
        //Role
        public const string Role_SUPERADMIN = "SUPERADMIN";
        public const string Role_ADMIN = "ADMIN";
        public const string Role_BRANCHMANAGER = "BRANCHMANAGER";
        public const string Role_CASHIER = "CASHIER";
        public const string Role_SUPERVISOR = "SUPERVISOR";

        public static List<string> RoleList =
        new List<string>{
            Role_SUPERADMIN, Role_ADMIN, Role_BRANCHMANAGER, Role_CASHIER, Role_SUPERVISOR
        };

        public static int getRoleLevel(string role)
        {
            switch (role)
            {
                case AppConstant.Role_CASHIER: return 1;
                case AppConstant.Role_SUPERVISOR: return 2;
                case AppConstant.Role_BRANCHMANAGER: return 3;
                case AppConstant.Role_ADMIN: return 4;
                case AppConstant.Role_SUPERADMIN: return 5;
                default: return 0;
            }
        }

        //##########################################################
        //LokasiPelabuhan

        public static List<string> LokasiPelabuhan =
        new List<string>{
            "BAKAUHENI", "BOJONEGARA"
        };


        public static class Permission  {
            public const string TicketView = "TicketView";
            public const string TicketCreate = "TicketCreate";
            public const string TicketUpdate = "TicketUpdate";
            public const string TicketUpdateBeforePrint = "TicketUpdateBeforePrint";
            public const string TicketPrint = "TicketPrint";

            public const string TicketDelete = "TicketDelete";
            public const string ManifestView = "ManifestView";
            public const string ManifestCreateUpdate = "ManifestCreateUpdate";
            public const string ManifestDelete = "ManifestDelete";
            public const string MasterGolonganView = "MasterGolonganView";
            public const string MasterGolonganManage = "MasterGolonganManage";
            public const string MasterPlatView = "MasterPlatView";
            public const string MasterPlatManage = "MasterPlatManage";
            public const string MasterUserView = "MasterUserView";
            public const string MasterUserManage = "MasterUserManage";
            public const string Report = "Report";


            static Dictionary<string, List<string>> permissionMapping = new Dictionary<string, List<string>>() {
                { Role_SUPERADMIN, new List<string>(){Permission.TicketCreate} },
                { Role_ADMIN, new List<string>{Permission.MasterGolonganManage,Permission.MasterUserManage,Permission.MasterPlatManage,Permission.TicketDelete,Permission.ManifestView}},
                { Role_BRANCHMANAGER, new List<string>{ Permission.MasterGolonganManage, Permission.MasterUserManage, Permission.MasterPlatManage, Permission.TicketView, Permission.ManifestCreateUpdate, Permission.TicketCreate, Permission.TicketUpdate}},
                { Role_SUPERVISOR, new List<string>{Permission.ManifestCreateUpdate,Permission.TicketView} },
                { Role_CASHIER, new List<string>{Permission.TicketCreate, Permission.TicketUpdateBeforePrint}},
            };


            public static string getPermissionString(string role)
            {
                List<string> result = new List<string>();
                if (role == Role_SUPERADMIN)
                {
                    var keys = typeof(Permission).GetFields();
                    foreach (var s in keys)
                    {
                        result.Add((string) s.GetValue(null));
                    }
                    return String.Join(",", result);
                }

                permissionMapping.TryGetValue(role, out result);

                return String.Join(",",result);
            }
        }

    }
}
