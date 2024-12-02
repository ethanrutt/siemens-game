import globals from "globals";
import pluginJs from "@eslint/js";


/** @type {import('eslint').Linter.Config[]} */
export default [
  {languageOptions: { globals: globals.browser }},
  pluginJs.configs.recommended,
];
// .eslintrc.json
{
  "env": {
    "node": true,
    "jest": true
  },
  "extends": [
    "eslint:recommended",
    "plugin:jest/recommended"
  ],
  "plugins": ["jest"],
  "rules": {
    // Customize rules as needed
  }
}
