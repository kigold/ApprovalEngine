import { Notify } from 'quasar';

export const Toast = (message: string) => {
  Notify.create({ type: 'negative', message: message });
};
