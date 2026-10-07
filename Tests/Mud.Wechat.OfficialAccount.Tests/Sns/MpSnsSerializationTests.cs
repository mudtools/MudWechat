// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Sns;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Sns;

/// <summary>
/// 网页授权（sns）域 DTO 与官方报文的双向映射（用官方页给出的<b>原文示例</b>作为夹具）。
/// </summary>
public class MpSnsSerializationTests
{
    /// <summary>S1：sns/oauth2/access_token 官方返回示例解析（无 scope 字段）。</summary>
    [Fact]
    public void SnsAccessTokenResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"access_token":"ACCESS_TOKEN","expires_in":7200,"refresh_token":"REFRESH_TOKEN",
            "openid":"OPENID","unionid":"UNIONID","is_snapshotuser":1}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SnsJsonContext.Default.MpSnsAccessTokenResponse);

        response!.IsSuccess.Should().BeTrue();
        response.AccessToken.Should().Be("ACCESS_TOKEN");
        response.ExpiresIn.Should().Be(7200);
        response.RefreshToken.Should().Be("REFRESH_TOKEN");
        response.OpenId.Should().Be("OPENID");
        response.UnionId.Should().Be("UNIONID", "仅 snsapi_userinfo 作用域返回（官方示例携带）");
        response.IsSnapshotUser.Should().Be(1);
    }

    /// <summary>S2：sns/oauth2/refresh_token 官方返回示例解析（含 scope）。</summary>
    [Fact]
    public void SnsRefreshTokenResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"access_token":"ACCESS_TOKEN","expires_in":7200,"refresh_token":"REFRESH_TOKEN",
            "openid":"OPENID","scope":"SCOPE"}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SnsJsonContext.Default.MpSnsRefreshTokenResponse);

        response!.IsSuccess.Should().BeTrue();
        response.Scope.Should().Be("SCOPE", "refresh_token 响应含 scope（与换凭证响应表不同，各自页面原文）");
        response.OpenId.Should().Be("OPENID");
    }

    /// <summary>S3：sns/auth 两种官方响应形态（有效 / 无效）。</summary>
    [Fact]
    public void SnsAuthResponse_ShouldParseBothOfficialSamples()
    {
        var valid = System.Text.Json.JsonSerializer.Deserialize(
            """{"errcode":0,"errmsg":"ok"}""", SnsJsonContext.Default.MpSnsAuthResponse);
        valid!.IsSuccess.Should().BeTrue("有效凭证 errcode = 0");

        var invalid = System.Text.Json.JsonSerializer.Deserialize(
            """{"errcode":40003,"errmsg":"invalid openid"}""", SnsJsonContext.Default.MpSnsAuthResponse);
        invalid!.IsSuccess.Should().BeFalse("无效凭证 errcode = 40003");
    }

    /// <summary>S4：sns/userinfo 官方返回示例解析（完整资料字段——与 /cgi-bin/user/info 停供字段集不同）。</summary>
    [Fact]
    public void SnsUserInfoResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"openid":"OPENID","nickname":"NICKNAME","sex":1,"province":"PROVINCE",
            "city":"CITY","country":"COUNTRY","headimgurl":"https://thirdwx.qlogo.cn/132",
            "privilege":["chinaunicom"],"unionid":"UNIONID"}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SnsJsonContext.Default.MpSnsUserInfoResponse);

        response!.IsSuccess.Should().BeTrue();
        response.Nickname.Should().Be("NICKNAME", "sns/userinfo 在有效授权下仍返回昵称（与基础信息接口停供不同）");
        response.Sex.Should().Be(1);
        response.HeadImgUrl.Should().Be("https://thirdwx.qlogo.cn/132");
        response.Privilege.Should().Contain("chinaunicom");
        response.UnionId.Should().Be("UNIONID", "绑定开放平台账号后出现");
    }
}
