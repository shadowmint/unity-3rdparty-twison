# 3rdparty-twison

This is a unity package to support Twine exports in twison format, see https://github.com/lazerwalker/twison

## Macros, etc.

Macros and scripting are defined by the file format, and twison doesn't support them.

This library purely makes the data available; you cannot run a story with scripts in unity using this.

See the examples; if you want to just 'run' your twine story in unity, look at https://github.com/daterre/Cradle,
which supports the standard formats like Harlowe.

## Usage

See the tests in the `Editor/` folder for each class for usage examples.

## Install

From your unity project folder:

    npm init
    npm install unity-3rdparty-twison --save
    echo Assets/pkg-all >> .gitignore
    echo Assets/pkg-all.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/pkg-all folder.

## Development

    cd test
    npm install

To reinstall the files from the src folder, run `npm install ..` again.

### Tests

All tests are located in the `Tests` folder.
