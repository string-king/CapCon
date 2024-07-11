import type { ETransactionType } from "./enums/ETransactionType";

export interface ITransaction {
    id?: string;
    loanId: string;
    transactionType: ETransactionType;
    date: Date;
    amount: number;
    comment: string;
}