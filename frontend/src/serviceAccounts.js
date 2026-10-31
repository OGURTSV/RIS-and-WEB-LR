// Справочник сервисных учётных записей — тестовые данные вместо
// GET /api/service-accounts (docs/api-contract.md, 5.6).
// riskLevel и riskFactors потом будет считать сервер, здесь они уже готовые.

export const serviceAccounts = [
  { id: 1, samAccountName: 'svc_sql_1c', riskLevel: 'Low', riskFactors: [] },
  { id: 2, samAccountName: 'svc_web', riskLevel: 'Medium', riskFactors: ['PasswordNeverExpires'] },
  {
    id: 3,
    samAccountName: 'svc_backup',
    riskLevel: 'High',
    riskFactors: ['NoOwner', 'PasswordNeverExpires', 'Privileged'],
  },
]

// Задача хранит только serviceAccountId, имя учётки берём из справочника (3НФ)
export function accountName(id) {
  return serviceAccounts.find((account) => account.id === id)?.samAccountName ?? `#${id}`
}
