/**
 * @type {import('semantic-release').GlobalConfig}
 */
module.exports = {
  branches: ['main'],
  extends: ['semantic-release-commit-filter'],
  plugins: [
    '@semantic-release/commit-analyzer',
    '@semantic-release/release-notes-generator',
    '@semantic-release/github',
    [
      '@semantic-release/exec',
      {
        publishCmd:
          './scripts/set-build-info.sh ${nextRelease.version} ${branch.name} ${(new Date()).toISOString()}',
      },
    ],
  ],
};
