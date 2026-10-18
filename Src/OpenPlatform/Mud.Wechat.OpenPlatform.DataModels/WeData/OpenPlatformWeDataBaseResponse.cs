// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// We 分析基础应答（官方 <c>base_resp</c>，形状 <c>{ret, err_msg}</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/wedata_login.html"/></para>
/// <para>该信封仅出现在 <c>wedata_login</c> 应答内层，与其它端点的 <c>{errcode, errmsg}</c> 不同源。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataBaseResponse
{
    /// <summary>业务返回码（官方 <c>ret</c>，0 表示成功）。</summary>
    [JsonPropertyName("ret")]
    public int Ret { get; set; }

    /// <summary>业务错误信息（官方 <c>err_msg</c>）。</summary>
    [JsonPropertyName("err_msg")]
    public string? ErrorMessage { get; set; }
}
