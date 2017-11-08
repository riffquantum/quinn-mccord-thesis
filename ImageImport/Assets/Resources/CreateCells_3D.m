
% conn - the sqlite connection
% Cells - the list of new cells (structure)
% target - (optional) the table to insert into
% bDelete - clear out other segmentations from Cells.t
function trackIDs=CreateCells_3D(conn,Cells,bDelete)

trackIDs=[];
if isempty(Cells)
    return
end
% 
if (nargin>2 & bDelete)
    cmd=['delete from tblCells where time=',num2str(Cells(1).time)];
    exec(conn,cmd)
end
    
cmd=['ALTER TABLE tblCells ADD COLUMN maxRadius INTEGER channel INTEGER'];
conn.exec(cmd);

% if isfield(Cells,'cellFeatures')
%     cmd=['ALTER TABLE tblCells ADD COLUMN cellFeatures STRING'];
%     conn.exec(cmd);
%     cmd = ['insert into tblCells(time,trackID,centroid,surface,area,u16pixels,cellFeatures) values (?, ?, ?, ?, ?, ?,?)'];
% else
%     cmd = ['insert into tblCells(time,trackID,centroid,surface,area,u16pixels) values (?, ?, ?, ?, ?, ?)'];
% end

cmd = ['insert into tblCells(time,trackID,channel,maxRadius,centroid,area,verts,edges,normals,faces,u16pixels) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)'];
insertCells = conn.handle.prepareStatement(cmd);

% build list of cells first,
set(conn, 'AutoCommit', 'off');
for i=1:length(Cells)     
    insertCells=Write.prepareCell_3D(insertCells,Cells(i));
    Write.tryUpdateDB(insertCells);
end
Write.tryCommitDB(conn)
set(conn, 'AutoCommit', 'on');
insertCells.close();
   
% now, query back the trackIDs -1 from that time point 
cmdSql=['select cellID from tblCells WHERE trackID=-1;'];

Q=fetch(conn,cmdSql);
if isempty(Q)
cellIDs = [Cells(:).trackID];
else
cellIDs = [Q{:,1}];
end 

% return value
trackIDs=cellIDs;

cmdSql='SELECT COUNT(ROWID) FROM tblColors';
Q=fetch(conn,cmdSql);
nColors=Q{1};

set(conn, 'AutoCommit', 'off');
cmd = 'INSERT OR REPLACE INTO tblTracks (trackID,idxColor) VALUES(?, ?)';
insertTracks = conn.handle.prepareStatement(cmd);

% make new track structs for our new cells
% just colors for now
% match database generated hull ids back to our cells we're insertings
for i=1:length(cellIDs)
    idxColor = 1+round((nColors-1)*rand());
    cellID = int32(cellIDs(i));
    % note -- new tracks get the cells id to start
    % every cell initially solo on its track
    insertTracks.setInt(1,cellID);
    insertTracks.setInt(2,idxColor);
    Write.tryUpdateDB(insertTracks);
end
commit(conn);
set(conn, 'AutoCommit', 'on');
insertTracks.close();

% finally update the trackIDs in the table to match cell ids for new cells
cmdSql=['UPDATE tblCells SET trackID=cellID WHERE trackID=-1;'];
exec(conn,cmdSql);

