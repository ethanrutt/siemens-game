export default {
    transform: {},
    testEnvironment: 'node',
    moduleNameMapper: {
      '^./shared/utils.mjs$': '<rootDir>/../shared/utils.mjs', // Map import paths for Jest
    },
  };

