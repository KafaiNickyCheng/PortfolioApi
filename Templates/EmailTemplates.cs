using portfolio_api.Models;

namespace portfolio_api.Templates;

public static class EmailTemplates
{
    public static string ContactEmail(ContactRequest request)
    {
        var company = string.IsNullOrEmpty(request.Company) ? "—" : request.Company;

        return $"""
            <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #7c3aed; margin-bottom: 1.5rem;">
                    New Portfolio Message
                </h2>
                <table style="width:100%; border-collapse: collapse;">
                    <tr>
                        <td style="padding: 8px 0; color: #666; width: 100px;">Name</td>
                        <td style="padding: 8px 0;"><strong>{request.Name}</strong></td>
                    </tr>
                    <tr>
                        <td style="padding: 8px 0; color: #666;">Email</td>
                        <td style="padding: 8px 0;"><strong>{request.Email}</strong></td>
                    </tr>
                    <tr>
                        <td style="padding: 8px 0; color: #666;">Company</td>
                        <td style="padding: 8px 0;"><strong>{company}</strong></td>
                    </tr>
                </table>
                <hr style="margin: 1.5rem 0; border-color: #eee;" />
                <p style="color: #666; margin-bottom: 0.5rem;">Message:</p>
                <p style="background: #f9f9f9; padding: 1rem; border-radius: 8px; line-height: 1.7; color: #333;">
                    {request.Message}
                </p>
                <p style="color: #999; font-size: 0.8rem; margin-top: 2rem;">
                    Sent from your portfolio contact form
                </p>
            </div>
        """;
    }
}