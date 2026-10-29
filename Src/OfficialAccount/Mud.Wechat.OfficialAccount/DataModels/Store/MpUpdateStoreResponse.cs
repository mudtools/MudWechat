// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 更新门店信息（<c>POST /wxa/update_store</c>）响应。
/// </summary>
/// <remarks>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40097</c>（invalid args）/ <c>65115</c>（poi_id is not exist）/
/// <c>65118</c>（store status is invalid，该门店状态不允许更新）/ <c>85053</c>（please apply merchant first）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpUpdateStoreResponse : MpResponse
{
    /// <summary>获取或设置更新信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public MpStoreUpdateResult? Data { get; set; }
}
