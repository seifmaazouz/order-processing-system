CREATE OR REPLACE FUNCTION prevent_negative_quantity()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.Quantity < 0 THEN
        RAISE EXCEPTION 'Book quantity cannot be negative';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_prevent_negative_quantity
BEFORE UPDATE OF Quantity
ON Book
FOR EACH ROW
EXECUTE FUNCTION prevent_negative_quantity();

CREATE OR REPLACE FUNCTION auto_order_on_threshold()
RETURNS TRIGGER AS $$
DECLARE
    order_quantity INT := 50;
BEGIN
    -- Only trigger when quantity drops from above threshold to below threshold
    IF NEW.Quantity < NEW.Threshold AND OLD.Quantity >= OLD.Threshold THEN
        -- Create automatic admin order
        INSERT INTO AdminOrder(OrderDate, "Status", TotalPrice, PubID, ConfirmedBy)
        VALUES (CURRENT_DATE, 'Pending', order_quantity * NEW.SellingPrice, NEW.PubID, NULL);

        -- Create admin order item
        INSERT INTO AdminOrderItem(ISBN, OrderNum, Quantity, UnitPrice)
        VALUES (NEW.ISBN, currval(pg_get_serial_sequence('AdminOrder','OrderID')), order_quantity, NEW.SellingPrice);

    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_auto_order_on_threshold
AFTER UPDATE OF Quantity
ON Book
FOR EACH ROW
EXECUTE FUNCTION auto_order_on_threshold();