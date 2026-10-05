// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.PromotionQrCode;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「推广二维码」域服务商通道「企业注册」第三方应用 SDK。
/// <para>官方仅在第三方应用开发文档树下开放本族端点（服务商凭自身 provider_access_token
/// 以推广包引导企业注册），全部 2 个端点声明于本接口；自建 / 代开发应用官方不开放，不设对应子接口。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。</para>
/// <para>官方权限口径：本族端点页面未列独立权限范围，仅要求使用服务商的 provider_access_token。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "PromotionQrCode",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkServicePromotionQrCodeService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyServicePromotionQrCodeService : IWechatWorkServicePromotionQrCodeService
{
    /// <summary>
    /// 获取注册码
    /// <para>该 API 用于根据注册推广包生成注册码（register_code）。</para>
    /// <para>官方限制：register_code 只能消费一次，在访问注册链接时消费，最长为 512 个字节；
    /// expires_in 为其有效期，生成链接需要在有效期内点击跳转（官方示例值 600）。</para>
    /// <para>官方限制：state 只支持英文字母和数字，最长为 128 字节；若指定该参数，
    /// 「查询注册状态」接口及「注册完成回调事件」会相应返回该字段值。</para>
    /// <para>官方限制：follow_user 必须是服务商所在企业的成员；若配置该值，则由该注册码创建的企业，
    /// 在服务商管理后台的报备记录会自动标注跟进人员为指定成员。</para>
    /// <para>官方说明：若传递 corp_name / admin_name / admin_mobile，
    /// 进入注册企业填写信息页面时相应字段会自动填入表格。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRegisterCodeRequest"/>：template_id 推广包 ID / corp_name 企业名称 /
    /// admin_name 管理员姓名 / admin_mobile 管理员手机号 / state 用户自定义状态值 / follow_user 跟进人 userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>注册码（register_code，只能消费一次，最长 512 字节）与有效期（expires_in，秒）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90581"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/get_register_code")]
    Task<GetRegisterCodeResponse> GetRegisterCodeAsync(
        [Body] GetRegisterCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询注册状态
    /// <para>该 API 用于查询通过注册定制化新创建的企业注册状态，企业注册成功返回注册信息。</para>
    /// <para>官方限制：register_code 生成后的查询有效期为 24 小时。</para>
    /// <para>官方限制：仅支持「注册完成回调事件」或「获取注册码」接口返回的 register_code 调用。</para>
    /// <para>官方错误码：84024 非全新创建企业，不支持使用该接口查询。</para>
    /// <para>官方说明：contact_sync 仅当注册推广包开启通讯录迁移接口时返回；其中的 access_token 是
    /// 通讯录 API 接口调用凭证（有全部通讯录读写权限，官方特别提示「请注意与 provider_access_token 的区别」），
    /// 须由调用方自行保存，并显式传入 <see cref="IWechatWorkPromotionQrCodeContactSyncService"/> 的通讯录迁移端点
    /// ——SDK 令牌基座不缓存该凭证。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRegisterInfoRequest"/>：register_code 查询的注册码，官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>注册信息（corpid 企业 corpid / contact_sync 通讯录迁移凭证 / auth_user_info 授权管理员信息 /
    /// state 用户自定义状态值 / template_id 推广包 ID）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90582"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/get_register_info")]
    Task<GetRegisterInfoResponse> GetRegisterInfoAsync(
        [Body] GetRegisterInfoRequest request,
        CancellationToken cancellationToken = default);
}
