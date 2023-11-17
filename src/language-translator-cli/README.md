# language-translator-cli

This project converts text for you from one file to another to the format in your expected

Ensure you have configured your AWS credentials locally so that we can call the AWS Translate service to translate your json file

put your english file in the folder path `src/language-translator-cli/Assets/en.json`

run the app

You should get generated json files for the various specific languages in the path

```
/src/language-translator-cli/bin/Debug/net8.0/Assets/Generated
```

This app is mainly to generate language files meant to be used by the `react-intl` npm package for your react app
