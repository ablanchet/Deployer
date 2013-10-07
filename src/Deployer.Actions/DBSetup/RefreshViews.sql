declare @bigRefresh nvarchar(max);
set @bigRefresh = '';
select @bigRefresh = @bigRefresh + 'exec sp_refreshview ''['+s.name+'].['+v.name+']'' ' from sys.views v
	inner join sys.schemas s on s.schema_id = v.schema_id;

exec(@bigRefresh);