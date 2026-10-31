// Подписи кодов на экране — словарь из docs/data-model.md.
// В API и БД хранятся коды (New, High…), пользователю показываем русские подписи.

export const statusLabels = {
  New: 'Зарегистрирована',
  InProgress: 'В работе',
  Closed: 'Риск устранён',
  Cancelled: 'Отменена',
}

export const riskLabels = {
  Low: 'Низкий',
  Medium: 'Средний',
  High: 'Высокий',
}

export const riskFactorLabels = {
  NoOwner: 'нет владельца',
  PasswordNeverExpires: 'бессрочный пароль',
  Privileged: 'привилегированная учётная запись',
}
