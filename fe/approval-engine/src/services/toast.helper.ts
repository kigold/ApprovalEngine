import { Notify } from 'quasar';

export const Toast = (message: string) => {
  Notify.create({ type: 'negative', message: message });
};

export const HandleError = (message: string, errorObject: errorObject) => {
  Toast(message);
  errorObject.message = message;
};

export interface errorObject {
  message: string;
}
