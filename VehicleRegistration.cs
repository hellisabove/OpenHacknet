using System;

namespace Hacknet
{
	// Token: 0x0200018A RID: 394
	public class VehicleRegistration
	{
		// Token: 0x06000A08 RID: 2568 RVA: 0x000A11A2 File Offset: 0x0009F3A2
		public VehicleRegistration()
		{
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x000A11AD File Offset: 0x0009F3AD
		public VehicleRegistration(VehicleType vehicleType, string plate, string regNumber)
		{
			this.vehicle = vehicleType;
			this.licencePlate = plate;
			this.licenceNumber = regNumber;
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x000A11D0 File Offset: 0x0009F3D0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.vehicle.maker,
				" ",
				this.vehicle.model,
				" | Plate: ",
				this.licencePlate,
				" Licence: ",
				this.licenceNumber
			});
		}

		// Token: 0x04000B5B RID: 2907
		public VehicleType vehicle;

		// Token: 0x04000B5C RID: 2908
		public string licencePlate;

		// Token: 0x04000B5D RID: 2909
		public string licenceNumber;
	}
}
