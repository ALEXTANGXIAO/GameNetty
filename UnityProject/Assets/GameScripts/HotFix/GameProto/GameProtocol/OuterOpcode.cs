namespace GameNetty
{
	public static partial class OuterOpcode
	{
		 public const int C2R_RegisterRequest = 110000001;
		 public const int R2C_RegisterResponse = 160000001;
		 public const int C2R_LoginRequest = 110000002;
		 public const int R2C_LoginResponse = 160000002;
		 public const int C2G_LoginGateRequest = 110000003;
		 public const int G2C_LoginGateResponse = 160000003;
		 public const int C2G_CreateCharacterRequest = 110000004;
		 public const int G2C_CreateCharacterResponse = 160000004;
		 public const int C2G_EnterMapRequest = 110000005;
		 public const int G2C_EnterMapResponse = 160000005;
		 public const int G2M_CreateUnitRequest = 200000001;
		 public const int M2G_CreateUnitResponse = 250000001;
		 public const int C2M_ExitRequest = 200000002;
		 public const int M2C_ExitResponse = 250000002;
		 public const int C2M_MoveMessage = 190000001;
		 public const int M2C_MoveBroadcast = 190000002;
		 public const int H_C2G_Message = 100000001;
		 public const int H_C2G_MessageRequest = 110000006;
		 public const int H_G2C_MessageResponse = 160000006;
		 public const int H_C2G_PushMessageToClient = 100000002;
		 public const int H_G2C_ReceiveMessageToServer = 100000003;
		 public const int H_C2G_LoginAddressRequest = 110000007;
		 public const int H_G2C_LoginAddressResponse = 160000007;
		 public const int H_C2M_Message = 190000003;
		 public const int H_C2M_MessageRequest = 200000003;
		 public const int H_M2C_MessageResponse = 250000003;
		 public const int H_C2M_PushAddressMessageToClient = 190000004;
		 public const int H_M2C_ReceiveAddressMessageToServer = 190000005;
		 public const int H_C2G_LoginRequest = 110000008;
		 public const int H_G2C_LoginResponse = 160000008;
		 public const int H_C2G_RegisterRequest = 110000009;
		 public const int H_G2C_RegisterResponse = 160000009;
		 public const int CmdGmReq = 110000010;
		 public const int CmdGmRes = 160000010;
	}
}
