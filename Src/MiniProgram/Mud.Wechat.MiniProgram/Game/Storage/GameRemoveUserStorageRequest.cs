// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>删除已上报 KV 数据请求。官方参数名是单数 key（不是 keys），且为字符串数组。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.removeUserStorage.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameRemoveUserStorageRequest
{
    /// <summary>要删除的数据 key 列表（官方请求示例 ["gold", "score"]）。</summary>
    [JsonPropertyName("key")]
    public List<string>? Key { get; set; }
}
