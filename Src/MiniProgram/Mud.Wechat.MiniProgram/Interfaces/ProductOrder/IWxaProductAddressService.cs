// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ProductOrder;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」·收货 / 发货地址域（4 端点：
/// 新增 / 删除 / 查询 / 更新；路由族 <c>/product/address/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>地址簿是商家侧共享资源，不挂订单</b>：新增 / 更新写入的是商家地址簿（发货地址 <c>send_addr</c>、
/// 收货地址 <c>recv_addr</c>、同城配送与自提开关），下单时再由订单侧引用 <c>address_id</c>；
/// 因此本域四支与订单状态机<b>无</b>联动，删除被订单引用的地址也不会回滚已成立的订单。
/// </para>
/// <para>
/// <b>增改共用请求体</b>：官方 <c>add</c> 与 <c>update</c> 参数表逐字段同构，仅 <c>address_id</c>
/// 在 <c>update</c> 必填、<c>add</c> 缺省 ⇒ 共用 <see cref="WxaProductAddressUpsertRequest"/>，
/// 守卫按「同一 DTO 不得分叉出两份近似类型」锁定。
/// </para>
/// <para>
/// <b>个人信息红线</b>：地址簿含收件人姓名、手机号、详细门牌 ⇒ 请求与应答数据一律禁入日志、
/// 遥测与异常消息。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本域 4 页官方正文<b>未给</b>端点级频率数字、权限集编号与云调用方法名
/// （2026-10-18 逐页取证）⇒ 不得凭第三方 SDK 注释或记忆补数。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductOrder", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductAddressService
{
    /// <summary>
    /// 新增地址（<c>POST /product/address/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/address/add_address.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>address_detail</c> 单对象包装），见 <see cref="WxaProductAddressUpsertRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方回包示例未给 <c>address_id</c>，新地址 id 须经 <c>get</c> 反查）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>address_detail.address_info</c> 内
    /// <c>user_name</c> / <c>province_name</c> / <c>city_name</c> / <c>county_name</c> /
    /// <c>detail_info</c> / <c>tel_number</c> 为必填。
    /// </para>
    /// <para>
    /// <b>默认地址互斥由微信侧裁决</b>：<c>default_send</c> / <c>default_recv</c> 置 1 会顶掉原默认项，
    /// 返回成功不代表并发写入下仍是默认 ⇒ 需要确定性时串行写并回查。
    /// </para>
    /// <para><b>敏感值</b>：姓名、手机号、详细地址禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/product/address/add")]
    Task<WxaResponse> AddAddressAsync(
        [Body] WxaProductAddressUpsertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除地址（<c>POST /product/address/delete</c>，官方「删除」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/address/delete_address.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>address_id</c> 必填），见 <see cref="WxaProductAddressDeleteRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>物理删除、不可撤销</b>：被在途订单引用的地址删除后订单侧保留快照，新下单不能再选该 id
    /// ⇒ 非幂等语义（重复删除返回错误而非成功），不得放进自动重试管道。
    /// </para>
    /// </remarks>
    [Post("/product/address/delete")]
    Task<WxaResponse> DeleteAddressAsync(
        [Body] WxaProductAddressDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询地址（<c>POST /product/address/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/address/get_address.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>address_id</c> 选填；缺省按官方语义返回地址簿），见 <see cref="WxaProductAddressGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>address_list[]</c>，见 <see cref="WxaProductAddressGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>回包恒为列表</b>：官方无单对象回包分支，按 id 查询命中也落在 <c>address_list</c> 单元素里；
    /// 无分页字段。
    /// </para>
    /// <para>
    /// <b>字段表含已废弃项（照录）</b>：<c>name</c> 在官方表中标注「已废弃」，宿主应读
    /// <see cref="WxaProductAddress"/> 的结构化地址字段。
    /// </para>
    /// <para><b>敏感值</b>：回包含完整收货信息，禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/product/address/get")]
    Task<WxaProductAddressGetResponse> GetAddressAsync(
        [Body] WxaProductAddressGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新地址（<c>POST /product/address/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/address/update_address.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>address_detail.address_id</c> 必填），见 <see cref="WxaProductAddressUpsertRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>整单覆盖</b>：官方语义为按 <c>address_id</c> 覆盖写，未携带的可选子项按缺省值落库
    /// ⇒ 更新前须先 <c>get</c> 取回全量再改，否则会静默清空同城配送 / 自提开关等字段。
    /// </para>
    /// <para><b>敏感值</b>：姓名、手机号、详细地址禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/product/address/update")]
    Task<WxaResponse> UpdateAddressAsync(
        [Body] WxaProductAddressUpsertRequest request,
        CancellationToken cancellationToken = default);
}
