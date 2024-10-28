export default {
    transform: {},
    testEnvironment: 'node',
    moduleNameMapper: {
      '^./shared/utils.mjs$': '<rootDir>/../shared/utils.mjs', // Map import paths for Jest
    },
  };

module.exports = {
  collectCoverage: true,
  collectCoverageFrom: ["**/*.mjs"],  // Adjust the pattern if needed
  coverageDirectory: "<rootDir>/coverage",
  testEnvironment: "node"
};
