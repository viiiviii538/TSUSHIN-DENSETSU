/**
 * 通信機器の要約情報をフロントエンドで扱うための型定義です。
 * C# 側の Domain/NetworkDevice.cs と対応づけて、画面表示や API 通信で迷わないようにします。
 * 実際の Web 実装ではこの型を import して利用してください。
 */
export type NetworkStatus = "online" | "offline" | "maintenance";

export interface NetworkDeviceSummary {
  /** 機器を一意に識別する ID。C# の NetworkDevice.Id に対応します。 */
  id: string;
  /** 表示名。C# の NetworkDevice.Name に対応します。 */
  name: string;
  /** 現在の状態。画面では色分けなどに活用できます。 */
  status: NetworkStatus;
}

/**
 * Storybook やモック画面で使い回せるサンプルデータです。
 * 数を増やしたり、別ファイルに分けたりしても構いません。
 */
export const demoDevices: NetworkDeviceSummary[] = [
  { id: "router-01", name: "本社ルーター", status: "online" },
  { id: "switch-05", name: "支店スイッチ", status: "maintenance" },
  { id: "ap-12", name: "工場アクセスポイント", status: "offline" }
];
