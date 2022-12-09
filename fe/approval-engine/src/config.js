export default {
  apiBaseUrl:
    process.env.NODE_ENV === 'production'
      ? 'https://sample.com/api/v1'
      : 'https://localhost:7287',
  pageSize: 3,
};
