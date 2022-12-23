import { QTableProps } from 'quasar';

export interface TableProps<T> {
  loading: boolean;
  data: T[];
  pagination: QTableProps['pagination'];
}
