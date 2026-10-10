// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「文档」模块管理文档内容域公共 SDK
/// （编辑文档内容 + 获取文档数据）。
/// <para>
/// 官方对三类应用开放完全一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocDocumentService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocDocumentService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocDocumentService"/>。
/// </para>
/// <para>管理文档族见 <see cref="IWechatWorkWedocService"/>；
/// 管理表格内容族见 <see cref="IWechatWorkWedocSpreadsheetService"/>；
/// 管理智能表格内容族见 <see cref="IWechatWorkWedocSmartSheetService"/>；
/// 管理智能文档内容族见 <see cref="IWechatWorkWedocSmartDocService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与代开发自建应用需具有「文档」权限。编辑文档内容为批量更新形态：
/// 批量更新请求中若有一个操作报错则全部更新操作不生效；每次更新前需先调用获取文档数据接口获取各节点最新位置。</para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocDocumentService
{
    /// <summary>
    /// 编辑文档内容
    /// <para>该接口用于编辑指定文档的内容（批量更新操作：替换文本 / 插入文本 / 删除内容 / 插入图片 /
    /// 插入分页符 / 插入表格 / 插入段落 / 更新文本属性）。</para>
    /// <para>官方业务限制：单次批量更新操作数量 &lt;= 30；批量更新请求中若有一个操作报错则全部更新操作不生效；
    /// 要更新的文档版本与最新文档版本相差不能超过 100 个；每次更新前需先调用获取文档数据接口获取节点位置；
    /// 插入表格行数 &lt;= 100、列数 &lt;= 60、单元格总数 &lt;= 1000；ranges 个数不超过 10。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchUpdateWedocDocumentRequest"/>：docid / verison / requests）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97626"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98027"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98034"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/document/batch_update")]
    Task<WechatWorkResponse> BatchUpdateDocumentAsync(
        [Body] BatchUpdateWedocDocumentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文档数据
    /// <para>该接口用于获取指定文档的内容数据（文档内容根节点 Node 树与文档版本号）。</para>
    /// <para>官方业务限制：无额外业务限制（编辑文档内容前需先调用本接口获取各节点最新位置）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocDocumentDataRequest"/>：docid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文档版本与文档内容根节点（version / document）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101161"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101188"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101170"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/document/get")]
    Task<GetWedocDocumentDataResponse> GetDocumentDataAsync(
        [Body] GetWedocDocumentDataRequest request,
        CancellationToken cancellationToken = default);
}
