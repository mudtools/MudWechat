// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 分配高级功能账号响应体（<c>/cgi-bin/wedoc/vip/batch_add</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class BatchAddWedocVipResponse : WechatWorkResponse
{
    /// <summary>获取或设置分配成功的成员账号列表（官方字段名 <c>succ_userid_list</c>，succ 而非 success；官方口径：<b>已经是高级功能账号的 userid 也会出现在该列表</b>，故不可据其判断「本次新分配」）。</summary>
    [JsonPropertyName("succ_userid_list")]
    public List<string>? SuccUseridList { get; set; }

    /// <summary>获取或设置分配失败的成员账号列表。</summary>
    [JsonPropertyName("fail_userid_list")]
    public List<string>? FailUseridList { get; set; }
}
