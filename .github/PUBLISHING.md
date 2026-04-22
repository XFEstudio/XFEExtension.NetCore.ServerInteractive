# Publishing Packages

This repository now includes two GitHub Actions workflows:

- `Validate`: runs on `push` and `pull_request` to ensure the solution restores and builds cleanly.
- `Publish Packages`: runs when you push a tag that starts with `v`, for example `v3.0.9`, or when you trigger it manually from the Actions page.

## Required secrets

Add this repository secret before publishing to NuGet.org:

- `NUGET_API_KEY`: API key from NuGet.org with push permission for `XFEExtension.NetCore.ServerInteractive`

No extra secret is required for GitHub Packages. The workflow uses the built-in `GITHUB_TOKEN` and requests `packages: write`.

## Recommended release flow

1. Update the project and verify CI is green.
2. Push a version tag such as `v3.0.9`.
3. GitHub Actions will build, pack, upload the generated `.nupkg` as an artifact, then publish it to:
   - NuGet.org
   - GitHub Packages at `https://nuget.pkg.github.com/XFEstudio/index.json`

## Manual publish

If you need to republish with a specific version from the GitHub Actions UI:

1. Open `Publish Packages`.
2. Click `Run workflow`.
3. Fill in the `version` input.
4. Optionally set `skip_nuget_org` to `true` when you only want GitHub Packages.
