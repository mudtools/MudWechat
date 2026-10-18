// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 订单路径信息（<c>getorderpathinfo</c> 应答 <c>msg</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getOrderPathInfo.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpOrderPathInfo
{
    /// <summary>订单页面路径（官方 <c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面截图列表（官方 <c>img_list</c>，array）。</summary>
    [JsonPropertyName("img_list")]
    public List<string>? ImgList { get; set; }

    /// <summary>页面演示视频（官方 <c>video</c>）。</summary>
    [JsonPropertyName("video")]
    public string? Video { get; set; }

    /// <summary>测试账号（官方 <c>test_account</c>）。</summary>
    [JsonPropertyName("test_account")]
    public string? TestAccount { get; set; }

    /// <summary>测试账号密码（官方 <c>test_pwd</c>）。</summary>
    [JsonPropertyName("test_pwd")]
    public string? TestPwd { get; set; }

    /// <summary>测试账号备注（官方 <c>test_remark</c>）。</summary>
    [JsonPropertyName("test_remark")]
    public string? TestRemark { get; set; }

    /// <summary>路径状态（官方 <c>status</c>，number）。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>申请时间（官方 <c>apply_time</c>，number 时间戳）。</summary>
    [JsonPropertyName("apply_time")]
    public long? ApplyTime { get; set; }
}
