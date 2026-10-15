// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>上报用户数据 KV 项。托管数据限制见接口注释（单对 ≤1024 字节、单 key ≤128 字节、每 openid ≤128 对）。官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserStorage.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameSetUserStorageRequestKvListItem
{
    /// <summary>数据的 key，官方原文「目前可以为 '1' - '50'」（单 key 长度 ≤128 字节，超限官方回 87019）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>数据的 value。<b>官方两处口径并存</b>：参数表类型列标 number，而「将排行榜显示在小游戏中心」一节要求 value 是含 <c>wxgame.score</c>（Int32，必填）+ <c>wxgame.update_time</c>（Int64，必填）的 JSON Object 序列化字符串（<c>wxgame</c> 下不得自定义字段，同级可自定义）⇒ 取 <c>JsonElement?</c> 由宿主决定形态，不做类型推断。</summary>
    [JsonPropertyName("value")]
    public JsonElement? Value { get; set; }
}
