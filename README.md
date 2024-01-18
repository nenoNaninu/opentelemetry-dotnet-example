# opentelemetry-dotnet-example

## Table of Contents
- [スライド](#スライド)
- [動かし方](#動かし方)

## スライド

[![](https://github.com/nenoNaninu/opentelemetry-dotnet-example/assets/27144255/8e2d5652-308b-4a3b-967b-e7444f1a77b5)](https://speakerdeck.com/nenonaninu/c-number-dehazimeru-opentelemetry)


## 動かし方

docker compose で Opentelemetry Collector とか Grafana とかいろいろ立ち上げます。

Visual Studio で docker-compose を選択して実行するのが簡単です。

![image](https://github.com/nenoNaninu/opentelemetry-dotnet-example/assets/27144255/ce9a562d-2efc-4d1e-963a-f9ff3069740f)

あるいは CLI でも。

```
$ docker compose -f docker-compose.yml build
$ docker compose -f docker-compose.yml up
```

立ち上がった `WebApi.csproj` を雑に叩いてテレメトリデータを生成しましょう。
最終的にそれらを Grafana で眺めると楽しいでしょう。

- WebApi
  - http://localhost:5149/swagger
  - http://localhost:5149/signalr-dev
- Grafana: 
  - http://localhost:3000

