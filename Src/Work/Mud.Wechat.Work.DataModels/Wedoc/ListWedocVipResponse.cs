// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 获取高级功能账号列表响应体（<c>/cgi-bin/wedoc/vip/list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class ListWedocVipResponse : WechatWorkResponse
{
    /// <summary>获取或设置成员账号 userid 列表（官方：企业当前已分配高级功能且在应用可见范围的账号列表；
    /// 官方对该字段的原文备注为「如果不需要获取分配某个应用高级功能的 userid，该参数非必填」）。</summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }

    /// <summary>获取或设置是否还有更多数据。
    /// <para><b>注意</b>：官方 99518 页的参数表<b>未</b>列出 has_more，续页判据是 next_cursor（无更多数据时官方返回空串）；
    /// 本字段位仅作兼容保留，<b>不得</b>据其判定分页终点。</para></summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置下一次请求的翻页标记（官方：无更多数据时返回空，故本字段即续页的唯一判据）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
