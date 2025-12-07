# Contributing Guidelines

## Purpose

This document explains project-level development and CI/CD conventions
contributors must follow to avoid common build and packaging failures.

## Workflow Reuse Guidelines

### Calling a Reusable Workflow

-   When invoking a reusable workflow using `uses:` at **job level**,
    **do NOT specify `runs-on`** in the caller.
-   The **reusable workflow must**:
    -   Declare `on: workflow_call`
    -   Define `runs-on` for its internal jobs
-   Inputs are passed via `with:`, and secrets via `secrets:`.

### Example: Caller (no `runs-on`)

``` yaml
jobs:
  call-build:
    uses: ./.github/workflows/cd-build-test-pack.yml
    with:
      version: ${{ github.ref_name }}
      configuration: Release
```

### Example: Reusable Workflow

``` yaml
on:
  workflow_call:
    inputs:
      version:
        required: true
        type: string
      configuration:
        required: true
        type: string
        default: Release

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
      # �
```

## Versioning and Git Tag Guidelines

-   Tags used for packaging and publishing---whether passed to
    `/p:Version=` or `dotnet pack`---**must be valid SemVer/NuGet
    version strings**.
-   **Do NOT include a leading `v`** when passing the version into
    `dotnet` commands.

### Valid Examples

-   `3.2.6`
-   `3.2.6-preview`
-   `3.2.6-preview20251130229`
-   `3.2.6-alpha.1+build.123`

### Invalid Examples

-   `v3.2.6-preview20251130229`
-   `3.2.6 preview`

### Recommended Tagging Practice

-   Tag with leading `v` locally if desired, but CI must sanitize it.

## CI Behavior and Version Sanitization

### Sanitization Example

``` yaml
- name: Sanitize version
  id: sanitize
  run: |
    v="${{ inputs.version }}"
    v="${v#v}"
    v="${v#V}"
    echo "sanitized_version=$v" >> "$GITHUB_OUTPUT"
```

Use via: `${{ steps.sanitize.outputs.sanitized_version }}`

## Troubleshooting

-   If CI reports "not a valid version string," check for leading `v` or
    invalid characters.

## Maintenance

-   Keep conventions aligned with GitHub Actions and NuGet rules.

## Contributing

You are more than welcome to contribute in one of the following ways:

1. Basic: Give input, and suggestions for improvement by creating an issue and labeling it https://github.com/itbackyard/CryptoNet/issues
2. Advance: if you have good knowledge of C# and Cryptography just grab one of the issues, or features, or create a new one and refactor and add a pull request.
3. Documentation: Add, update, or improve documentation, by making a pull request.

### How to contribute:

[Here](https://www.dataschool.io/how-to-contribute-on-github/) is a link to learn how to contribute if you are not aware of how to do it.
