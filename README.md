# opentelemetry-dotnet-example

## Table of Contents
- [スライド](#スライド)
- [動かし方](#動かし方)
- [既知の問題](#既知の問題)

## スライド



[![](https://github.com/nenoNaninu/opentelemetry-dotnet-example/assets/27144255/8e2d5652-308b-4a3b-967b-e7444f1a77b5)](https://speakerdeck.com/nenonaninu/c-number-dehazimeru-opentelemetry)


## 動かし方

docker compose で Opentelemetry Collector とか Grafana とかいろいろ立ち上げます。

```
$ docker compose build
$ docker compose up
```

そのうえで、`src\WebApi\WebApi.csproj` を**ローカルホストで動かします。**
動かし方は `dotnet run` でも Visual Studio でも Rider でも OK。

```
$ dotnet run --project src\WebApi\WebApi.csproj --launch-profile https
```

立ち上がった `WebApi.csproj` を雑に叩きまくる。

- WebApi
  - http://localhost:5149/swagger
  - http://localhost:5149/signalr-dev
  - https://localhost:7237/swagger
  - https://localhost:7237/signalr-dev
- Grafana: 
  - http://localhost:3000

## 既知の問題

`WebApi.csproj` を docker compose 内で動かしテレメトリデータを collector に送信する事は可能なのだが、何故かログの signal だけ collector が正常に処理してくれないので(collector が log の signal を backend に送信しない)。docker を使わずに普通に `WebApi.csproj` を動かし、localhost で collector につなげるとログも正常に処理される。
実際 docker compose 内で動かしている `GrpcService.csproj` からはメトリクスとトレースは collector から各 backend に送信されているが、ログだけが送信されない。
