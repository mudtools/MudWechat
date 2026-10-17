// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

/// <summary>
/// 「申请设置订单页 path 信息」批量申请体（<c>applysetorderpathinfo</c> 请求体 <c>batch_req</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/applySetOrderPathInfo.html"/>。
/// </summary>
/// <remarks><para>官方限制：<c>appid_list</c> 一次提交不超过 100 个 appid（超限报 <c>61042</c>）。</para></remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramTemplate")]
public class OpenPlatformMpOrderPathBatchReq
{
    /// <summary>申请提交的订单页 path（官方必填 <c>path</c>，string）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>申请提交的图片 url 列表，审核版会显示（官方选填 <c>img_list</c>，array）。</summary>
    [JsonPropertyName("img_list")]
    public List<string>? ImgList { get; set; }

    /// <summary>申请提交的视频 url，审核版会显示（官方选填 <c>video</c>，string）。</summary>
    [JsonPropertyName("video")]
    public string? Video { get; set; }

    /// <summary>申请提交的测试账号，审核版会显示（官方选填 <c>test_account</c>，string）。</summary>
    [JsonPropertyName("test_account")]
    public string? TestAccount { get; set; }

    /// <summary>申请提交的测试密码，审核版会显示（官方选填 <c>test_pwd</c>，string）。</summary>
    [JsonPropertyName("test_pwd")]
    public string? TestPwd { get; set; }

    /// <summary>申请提交的测试备注，审核版会显示（官方选填 <c>test_remark</c>，string）。</summary>
    [JsonPropertyName("test_remark")]
    public string? TestRemark { get; set; }

    /// <summary>申请提交的批量 appid 列表，一次不超过 100 个（官方必填 <c>appid_list</c>，array）。</summary>
    [JsonPropertyName("appid_list")]
    public List<string>? AppidList { get; set; }
}
