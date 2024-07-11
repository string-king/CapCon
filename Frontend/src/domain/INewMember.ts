import type { ECompanyRole } from "./enums/ECompanyRole";

export interface INewMember {
    companyId: string;
    email: string;
    role: ECompanyRole;
}