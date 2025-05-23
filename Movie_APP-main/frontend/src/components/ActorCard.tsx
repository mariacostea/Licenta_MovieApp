import React from "react"

interface Props {
    imageUrl?: string | null;
    firstName: string;
    lastName: string;
}

const ActorCard: React.FC<Props> = ({ imageUrl, firstName, lastName }) => (
    <div className="col-6 col-md-3 col-lg-12 mb-2">
        <div className="card bg-dark text-white h-100 border-0 shadow-sm">
            <img
                src={imageUrl ?? "https://via.placeholder.com/300x450?text=No+Image"}
                className="card-img-top"
                alt={`${firstName} ${lastName}`}
            />
            <div className="card-body p-2">
                <h6 className="card-title mb-0 text-center" style={{ fontSize: "0.9rem" }}>
                    {firstName} <br /> {lastName}
                </h6>
            </div>
        </div>
    </div>
);

export default ActorCard;
