Add migration dotnet ef migrations add Init --verbose -- "Data Source={0}; Initial Catalog={1}; TrustServerCertificate=True; Integrated Security=False; user={2}; password={3}"

Apply migration dotnet ef database update --verbose -- "Data Source={0}; Initial Catalog={1}; TrustServerCertificate=True; Integrated Security=False; user={2}; password={3}"