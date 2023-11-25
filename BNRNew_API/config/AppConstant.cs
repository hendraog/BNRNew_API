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

        
    }
}
